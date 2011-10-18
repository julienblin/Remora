using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Exceptions;
using Remora.Pipeline;

namespace Remora.Components
{
    public class Recorder : AbstractPipelineComponent
    {
        public const string ComponentId = @"recorder";

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
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Beginning the recording process for {0}...", operation);

            operation.ExecutionProperties["Recorder.Path"] = GetPath(componentDefinition);
            operation.ExecutionProperties["Recorder.Action"] = ExtractAction(operation);

            callback(true);
        }

        private string ExtractAction(IRemoraOperation operation)
        {
            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    return ExtractSoapAction(operation);
                default:
                    if(Logger.IsDebugEnabled)
                        Logger.DebugFormat("Unable to extract action from operation {0}.", operation);
                    return null;
            }
        }

        private string ExtractSoapAction(IRemoraOperation operation)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Extracting SOAP action from operation {0}...", operation);

            //var doc = XDocument.Load(operation.IncomingRequest.Data);
            throw new NotImplementedException();
        }

        protected virtual string GetPath(IComponentDefinition componentDefinition)
        {
            if (!componentDefinition.Properties.ContainsKey("path"))
            {
                throw new RecorderException("Unable to record interaction, because the path property is not set. You must add a path attribute to the <component id=\"recorder\" /> element in configuration.");
            }

            var path = componentDefinition.Properties["path"];

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    throw new RecorderException(string.Format("The path {0} doesn't exist and recorder was unable to create it.", path), ex);
                }
            }
            return path;
        }
    }
}
