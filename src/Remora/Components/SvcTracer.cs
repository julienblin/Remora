using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Remora.Configuration;
using Remora.Core;
using Remora.Pipeline;

namespace Remora.Components
{
    public class SvcTracer : AbstractPipelineComponent
    {
        public const string ComponentId = @"svcTracer";

        private readonly TraceSource _traceSource;

        public SvcTracer()
            : this("Remora")
        {
        }

        public SvcTracer(string traceSource)
        {
            _traceSource = new TraceSource(traceSource);
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            var activityId = GetActivityId(operation);
            _traceSource.TraceEvent(TraceEventType.Start, 0, string.Format("Processing incoming request from {0}.", operation.IncomingUri));
            callback(true);
        }

        private Guid GetActivityId(IRemoraOperation operation)
        {
            // TODO
            return Guid.NewGuid();
        }
    }
}
