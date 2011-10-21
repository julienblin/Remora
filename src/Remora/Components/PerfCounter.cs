using System;
using System.Diagnostics;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Pipeline;

namespace Remora.Components
{
    public class PerfCounter : AbstractPipelineComponent
    {
        public const string ComponentId = @"perfCounter";
        public const string DefaultPerfCounterCategory = @"Remora - Messages";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private PerfCounterFactory _perfCounterFactory;

        public PerfCounterFactory PerfCounterFactory
        {
            get { return _perfCounterFactory ?? (_perfCounterFactory = new PerfCounterFactory { Logger = Logger }); }
            set { _perfCounterFactory = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            var category = GetCategory(componentDefinition);
            var executionPropertyKey = GetStopwatchExecutionPropertyKey(category);
            if (operation.ExecutionProperties.ContainsKey(executionPropertyKey))
            {
                Logger.WarnFormat("There may exists two perfCounter with the same category ({0}) in the same pipeline. Results may be inacurate.", category);
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
                WritePerformanceCounters(operation, componentDefinition);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "There has been an error while processing performance counters for operation {0}.", operation);
            }
            callback();
        }

        private void WritePerformanceCounters(IRemoraOperation operation, IComponentDefinition componentDefinition)
        {
            var category = GetCategory(componentDefinition);
            var stopwatchExecutionProperty = GetStopwatchExecutionPropertyKey(category);
            if (operation.ExecutionProperties.ContainsKey(stopwatchExecutionProperty))
            {
                var stopwatch = (Stopwatch)operation.ExecutionProperties[stopwatchExecutionProperty];
                stopwatch.Stop();

                using (var numMessagesSent = PerfCounterFactory.GetPerfCounter(PerfCounterDefinition.NumMessagesHandled, operation.ExecutingPipeline.Id, category))
                    numMessagesSent.IncrementBy(1);

                using (var numMessagesSentPerSec = PerfCounterFactory.GetPerfCounter(PerfCounterDefinition.NumMessagesHandledPerSec, operation.ExecutingPipeline.Id, category))
                    numMessagesSentPerSec.IncrementBy(1);

                using (var averageDurationForMessageSending = PerfCounterFactory.GetPerfCounter(PerfCounterDefinition.AverageDurationForMessageHandling, operation.ExecutingPipeline.Id, category))
                    averageDurationForMessageSending.IncrementBy(stopwatch.ElapsedTicks);

                using (var averageDurationForMessageSendingBase = PerfCounterFactory.GetPerfCounter(PerfCounterDefinition.AverageDurationForMessageHandlingBase, operation.ExecutingPipeline.Id, category))
                    averageDurationForMessageSendingBase.IncrementBy(1);
            }
            else
            {
                Logger.WarnFormat("Unable to collect perf counters data for operation {0} because no stopwatch is found on the ExecutionProperties.", operation);
            }
        }

        private static string GetCategory(IComponentDefinition componentDefinition)
        {
            return componentDefinition.Properties.ContainsKey("category") ? componentDefinition.Properties["category"] : DefaultPerfCounterCategory;
        }

        private static string GetStopwatchExecutionPropertyKey(string category)
        {
            return string.Format("PerfCounter.{0}.Stopwatch", category);
        }
    }

    public enum PerfCounterDefinition
    {
        NumMessagesHandled,
        NumMessagesHandledPerSec,
        AverageDurationForMessageHandling,
        AverageDurationForMessageHandlingBase
    }

    public class PerfCounterFactory
    {
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        bool _testPerfCountersCreated;

        public PerformanceCounter GetPerfCounter(PerfCounterDefinition counter, string instanceName, string categoryName, bool readOnly = false)
        {
            if (!_testPerfCountersCreated)
            {
                try
                {
                    CreatePerfCounters(categoryName);
                }
                catch (Exception ex)
                {
                    Logger.WarnFormat(ex, "Unable to create performance counters for category {0}.", categoryName);
                    return null;
                }
                _testPerfCountersCreated = true;
            }

            var perfCounter = new PerformanceCounter();

            switch (counter)
            {
                case PerfCounterDefinition.NumMessagesHandled:
                    perfCounter.CounterName = @"# messages handled";
                    perfCounter.CategoryName = categoryName;
                    break;
                case PerfCounterDefinition.NumMessagesHandledPerSec:
                    perfCounter.CounterName = @"# messages handled / sec";
                    perfCounter.CategoryName = categoryName;
                    break;
                case PerfCounterDefinition.AverageDurationForMessageHandling:
                    perfCounter.CounterName = @"average time per message handling";
                    perfCounter.CategoryName = categoryName;
                    break;
                case PerfCounterDefinition.AverageDurationForMessageHandlingBase:
                    perfCounter.CounterName = @"average time per message handling base";
                    perfCounter.CategoryName = categoryName;
                    break;
            }

            perfCounter.MachineName = ".";
            perfCounter.InstanceName = instanceName;
            perfCounter.ReadOnly = readOnly;
            return perfCounter;
        }

        public void CreatePerfCounters(string categoryName)
        {
            if (!PerformanceCounterCategory.Exists(categoryName))
            {
                if(Logger.IsDebugEnabled)
                    Logger.DebugFormat("Creating perf counters for category {0}...", categoryName);

                var counters = new CounterCreationDataCollection
                                   {
                                       new CounterCreationData
                                           {
                                               CounterName = @"# messages handled",
                                               CounterHelp = @"Total number of messages handled",
                                               CounterType = PerformanceCounterType.NumberOfItems32
                                           },
                                       new CounterCreationData
                                           {
                                               CounterName = @"# messages handled / sec",
                                               CounterHelp = @"Number of messages handled per second",
                                               CounterType = PerformanceCounterType.RateOfCountsPerSecond32
                                           },
                                       new CounterCreationData
                                           {
                                               CounterName = @"average time per message handling",
                                               CounterHelp =
                                                   @"Average time spent handling (processing) a message",
                                               CounterType = PerformanceCounterType.AverageTimer32
                                           },
                                       new CounterCreationData
                                           {
                                               CounterName =
                                                   @"average time per message handling base",
                                               CounterHelp =
                                                   @"Average time spent handling (processing) a message base",
                                               CounterType = PerformanceCounterType.AverageBase
                                           }
                                   };

                PerformanceCounterCategory.Create(
                    categoryName,
                    "Counters for the Remora proxy server",
                    PerformanceCounterCategoryType.MultiInstance,
                    counters
                );
            }
        }
    }
}
