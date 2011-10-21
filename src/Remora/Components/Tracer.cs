#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Diagnostics;
using System.IO;
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
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition,
                                               Action<bool> callback)
        {
            var category = GetCategory(componentDefinition);
            var executionPropertyKey = GetStopwatchExecutionPropertyKey(category);
            if (operation.ExecutionProperties.ContainsKey(executionPropertyKey))
            {
                Logger.WarnFormat(
                    "There may exists two tracer with the same category ({0}) in the same pipeline. Results may be inacurate.",
                    category);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            operation.ExecutionProperties[executionPropertyKey] = stopwatch;
            callback(true);
        }

        public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition,
                                             Action callback)
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
                Logger.WarnFormat(
                    "Unable to trace operation {0}: no directory has been provided. You must use the directory attribute in the component configuration.",
                    operation);
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
                    Logger.WarnFormat(ex,
                                      "Unable to trace operation {0}: the directory {1} does not exists and there has been an error when creating it.",
                                      operation, directoryPath);
                    return;
                }
            }

            var fileName = Path.Combine(directoryPath,
                                        string.Format("{0}-{1}-{2}.xml",
                                                      DateTime.UtcNow.ToString("s").MakeValidFileName(),
                                                      category.MakeValidFileName(), operation.OperationId));

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
            return componentDefinition.Properties.ContainsKey("category")
                       ? componentDefinition.Properties["category"]
                       : "default";
        }

        private static string GetStopwatchExecutionPropertyKey(string category)
        {
            return string.Format("Tracer.{0}.Stopwatch", category);
        }
    }
}