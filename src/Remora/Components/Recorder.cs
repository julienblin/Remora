using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Pipeline;
using Remora.Transformers;

namespace Remora.Components
{
    public class Recorder : AbstractPipelineComponent
    {
        public const string ComponentId = @"recorder";

        private readonly ISoapTransformer _soapTransformer;
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public Recorder(ISoapTransformer soapTransformer)
        {
            if (soapTransformer == null) throw new ArgumentNullException("soapTransformer");
            Contract.EndContractBlock();

            _soapTransformer = soapTransformer;
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    RecordSoapOperation(operation);
                    break;
            }

            callback(true);
        }

        public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
        {
            if (!componentDefinition.Properties.ContainsKey("directory"))
            {
                Logger.WarnFormat("Unable to record operation {0}: no directory has been provided. You must use the directory attribute in the component configuration.", operation);
                callback();
                return;
            }
            var directoryPath = componentDefinition.Properties["directory"];

            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    Logger.WarnFormat(ex, "Unable to record operation {0}: the directory {1} does not exists and there has been an error when creating it.", operation, directoryPath);
                    callback();
                    return;
                }
            }

            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    EndRecordSoapOperation(directoryPath, operation);
                    break;
                default:
                    Logger.WarnFormat("Unable to record operation {0} because its kind ({1}) is unsupported.", operation, operation.Kind);
                    break;
            }
            callback();
        }

        private void RecordSoapOperation(IRemoraOperation operation)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Recording operation {0} as soap...", operation);

            var soapDoc = _soapTransformer.LoadSoapDocument(operation.Request);
            if (soapDoc == null)
            {
                Logger.WarnFormat("Unable to record operation {0}: the soap document cannot be loaded.", operation);
                return;
            }

            var soapAction = _soapTransformer.GetSoapActionName(soapDoc);
            if (soapAction == null)
            {
                Logger.WarnFormat("Unable to record operation {0}: the soap action name cannot be determined.", operation);
                return;
            }

            operation.ExecutionProperties["Recorder.SoapActionName"] = soapAction;
        }

        private void EndRecordSoapOperation(string directoryPath, IRemoraOperation operation)
        {
            var soapActionName = (string)operation.ExecutionProperties["Recorder.SoapActionName"];
            if (string.IsNullOrEmpty(soapActionName))
            {
                Logger.WarnFormat("Unable to complete the recording of operation {0}: the soap action name has not been set.", operation);
                return;
            }

            var fileName = Path.Combine(directoryPath, string.Format("{0}.xml", soapActionName));
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: saving record for {1} in {2}...", operation, soapActionName, fileName);

            var recordAction = new RecordAction
            {
                Headers = operation.Response.HttpHeaders.Select(k => new RecordActionHeader(k.Key, k.Value)).ToArray(),
                RequestContentEncoding = operation.Request.ContentEncoding.HeaderName,
                Request = Encoding.UTF8.GetString(operation.Request.Data),
                ResponseStatusCode = operation.Response.StatusCode,
                OnError = operation.OnError
            };

            if (operation.OnError)
            {
                // TODO
            }
            else
            {
                recordAction.ResponseContentEncoding = operation.Response.ContentEncoding.HeaderName;
                recordAction.Response = Encoding.UTF8.GetString(operation.Response.Data);
            }

            try
            {
                using (var writeStream = File.OpenWrite(fileName))
                {
                    var serializer = new DataContractSerializer(typeof (RecordAction));
                    serializer.WriteObject(writeStream, recordAction);
                }

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Operation {0}: succefully recorded in {1}.", operation, fileName);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "There has been an error while saving record file {0} for operation {1}.", fileName, operation);
            }
        }
    }
}
