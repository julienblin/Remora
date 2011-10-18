using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Components;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class PipelineEngine : IPipelineEngine
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

        public IKernel Kernel { get; set; }

        #region IPipelineEngine Members

        public void RunAsync(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            if (callback == null) throw new ArgumentNullException("callback");
            Contract.EndContractBlock();

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Running operation {0} on pipeline {1}...", operation, pipeline.Id);

            var topInvocation = BuildInvocations(operation, pipeline, callback);
            topInvocation.BeginProcess();
        }

        #endregion

        private IPipelineComponentInvocation BuildInvocations(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback)
        {
            var currentInvocation = new PipelineComponentInvocation { Logger = Logger, Operation = operation, Component = Kernel.Resolve<IPipelineComponent>(Sender.SenderComponentId)};

            foreach (var pipelineComponent in pipeline.Components.Reverse())
            {
                if (pipelineComponent != null)
                {
                    var previousInvocation = currentInvocation;
                    currentInvocation = new PipelineComponentInvocation
                    {
                        Logger = Logger,
                        Operation = operation,
                        Component = pipelineComponent,
                        NextInvocation = currentInvocation
                    };
                    previousInvocation.PreviousInvocation = currentInvocation;
                }
            }

            var finalInvocation = new FinalCallbackPipelineComponentInvocation { Logger = Logger, Operation = operation, NextInvocation = currentInvocation, FinalCallback = callback };
            currentInvocation.PreviousInvocation = finalInvocation;

            return finalInvocation;
        }
    }
}
