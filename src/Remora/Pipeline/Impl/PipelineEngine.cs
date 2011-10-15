using System;
using System.Diagnostics.Contracts;
using Castle.Core.Logging;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    /// <summary>
    /// Default implementation for <see cref="IPipelineEngine"/>
    /// </summary>
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

        public void Run(IRemoraOperation operation, IPipeline pipeline)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (pipeline == null) throw new ArgumentNullException("pipeline");
            Contract.EndContractBlock();

            try
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Running operation {0} on pipeline {1}...", operation, pipeline.Id);

                var topInvocation = BuildInvocationChain(operation, pipeline);
                topInvocation.ProceedWithNextComponent();

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Operation {0} on pipeline {1} has been executed successfully.", operation, pipeline.Id);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error while running operation {0} on pipeline {1}", operation, pipeline.Id);
                throw;
            }
        }

        private IPipelineComponentInvocation BuildInvocationChain(IRemoraOperation operation, IPipeline pipeline)
        {
            IPipelineComponentInvocation currentInvocation = new PipelineComponentInvocation(Logger, operation, pipeline, null, null);
            foreach (var pipelineComponent in pipeline.Components)
            {
                if (pipelineComponent != null)
                    currentInvocation = new PipelineComponentInvocation(Logger, operation, pipeline, pipelineComponent, currentInvocation);
            }

            return currentInvocation;
        }
    }
}
