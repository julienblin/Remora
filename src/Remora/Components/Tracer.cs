using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            var category = GetCategory(componentDefinition);
            var executionPropertyKey = GetStopwatchExecutionPropertyKey(category);
            if(operation.ExecutionProperties.ContainsKey(executionPropertyKey))
            {
                Logger.WarnFormat("There may exists two tracer with the same category ({0}) in the same pipeline. Results may be inacurate.", category);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            operation.ExecutionProperties[executionPropertyKey] = stopwatch;
            callback(true);
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
            var category = GetCategory(componentDefinition);

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

            var fileName = Path.Combine(directoryPath, string.Format("{0}-{1}-{2}.xml", DateTime.UtcNow.ToString("s").MakeValidFileName(), category.MakeValidFileName(), operation.OperationId));

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: saving trace ({1}) in {2}...", operation, category, fileName);

            var serializableOperation = new SerializableOperation(operation);

            var stopwatchExecutionProperty = GetStopwatchExecutionPropertyKey(category);
            if (operation.ExecutionProperties.ContainsKey(stopwatchExecutionProperty))
            {
                var stopwatch = (Stopwatch) operation.ExecutionProperties[stopwatchExecutionProperty];
                stopwatch.Stop();
                serializableOperation.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            using (var writeStream = File.OpenWrite(fileName))
            {
                serializableOperation.Serialize(writeStream);
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Operation {0}: successfully traced ({1}) in {1}.", operation, category, fileName);
        }

        private static string GetCategory(IComponentDefinition componentDefinition)
        {
            return componentDefinition.Properties.ContainsKey("category") ? componentDefinition.Properties["category"] : "default";
        }

        private static string GetStopwatchExecutionPropertyKey(string category)
        {
            return string.Format("Tracer.{0}.Stopwatch", category);
        }
    }
}
