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
using Remora.Core.Impl;
using Remora.Core.Serialization;
using Remora.Exceptions;
using Remora.Extensions;
using Remora.Pipeline;
using Remora.Transformers;

namespace Remora.Components
{
    public class SoapRecorder : AbstractPipelineComponent
    {
        public const string ComponentId = @"soapRecorder";

        public const string SoapActionKey = @"SoapRecorder.SoapAction";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            if (operation.Kind == RemoraOperationKind.Soap)
            {
                RecordSoapOperation(operation);
            }
            else
            {
                Logger.WarnFormat("Unable to record operation {0} because it appears to not be a soap request.", operation);
            }

            callback(true);
        }

        public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
        {
            if (operation.Kind == RemoraOperationKind.Soap)
            {
                EndRecordSoapOperation(operation, componentDefinition);
            }

            callback();
        }

        private void RecordSoapOperation(IRemoraOperation operation)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Recording operation {0} as soap...", operation);

            if (!operation.Request.HttpHeaders.ContainsKey("SOAPAction"))
            {
                Logger.WarnFormat("Unable to record operation {0} because it doesn't have a SOAPAction header.", operation);
                return;
            }

            operation.ExecutionProperties[SoapActionKey] = operation.Request.HttpHeaders["SOAPAction"];
        }

        private void EndRecordSoapOperation(IRemoraOperation operation, IComponentDefinition componentDefinition)
        {
            if (!operation.ExecutionProperties.ContainsKey(SoapActionKey))
                return;

            var soapActionName = (string)operation.ExecutionProperties[SoapActionKey];

            if (!componentDefinition.Properties.ContainsKey("directory"))
            {
                Logger.WarnFormat("Unable to record operation {0}: no directory has been provided. You must use the directory attribute in the component configuration.", operation);
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
                    return;
                }
            }

            var randomAppendToFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var fileName = Path.Combine(directoryPath, string.Format("{0}.{1}.xml", soapActionName.MakeValidFileName(), randomAppendToFileName));

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: saving record for {1} in {2}...", operation, soapActionName, fileName);

            var serializableOperation = new SerializableOperation(operation);

            try
            {
                using (var writeStream = File.OpenWrite(fileName))
                {
                    serializableOperation.Serialize(writeStream);
                }

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Operation {0}: successfully recorded in {1}.", operation, fileName);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "There has been an error while saving record file {0} for operation {1}.", fileName, operation);
            }
        }
    }
}
