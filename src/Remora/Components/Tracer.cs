using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Core.Serialization;
using Remora.Extensions;
using Remora.Pipeline;

namespace Remora.Components
{
    public class Tracer : AbstractPipelineComponent
    {
        public const string ComponentId = @"tracer";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
        {
            try
            {
                TraceOperation(operation, componentDefinition);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "There has been an error while tracing operation {0}.", operation);
            }
            callback();
        }

        private void TraceOperation(IRemoraOperation operation, IComponentDefinition componentDefinition)
        {
            if (!componentDefinition.Properties.ContainsKey("directory"))
            {
                Logger.WarnFormat("Unable to trace operation {0}: no directory has been provided. You must use the directory attribute in the component configuration.", operation);
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
                    Logger.WarnFormat(ex, "Unable to trace operation {0}: the directory {1} does not exists and there has been an error when creating it.", operation, directoryPath);
                    return;
                }
            }

            var fileName = Path.Combine(directoryPath, string.Format("{0}-{1}-{2}.xml", DateTime.UtcNow.ToString("s").MakeValidFileName(), operation.IncomingUri.ToString().MakeValidFileName(), operation.OperationId));

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: saving trace in {1}...", operation, fileName);

            var serializableOperation = new SerializableOperation(operation);
            using (var writeStream = File.OpenWrite(fileName))
            {
                serializableOperation.Serialize(writeStream);
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: successfully traced in {1}.", operation, fileName);
        }
    }
}
