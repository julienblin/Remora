using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class PipelineComponentInvocation : IPipelineComponentInvocation
    {
        private readonly ILogger _logger;
        private readonly IPipelineComponent _component;
        private readonly IPipelineComponentInvocation _nextInvocation;

        public PipelineComponentInvocation(ILogger logger, IRemoraOperation operation, IPipeline pipeline, IPipelineComponent component, IPipelineComponentInvocation nextInvocation)
        {
            _logger = logger;
            _component = component;
            _nextInvocation = nextInvocation;
            Operation = operation;
            Pipeline = pipeline;
        }

        public IRemoraOperation Operation { get; private set; }

        public IPipeline Pipeline { get; private set; }

        public void ProceedWithNextComponent()
        {
            if ((_component != null) && (_nextInvocation != null))
            {
                if (_logger.IsDebugEnabled)
                    _logger.DebugFormat("Proceeding with component {0} for operation {1} on pipeline {2}...", _component.Id, Operation, Pipeline.Id);

                _component.Proceed(_nextInvocation);

                if (_logger.IsDebugEnabled)
                    _logger.DebugFormat("Component {0} successfully executed operation {1} on pipeline {2}.", _component.Id, Operation, Pipeline.Id);
            }
        }
    }
}
