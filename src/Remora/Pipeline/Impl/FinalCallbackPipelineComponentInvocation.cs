using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class FinalCallbackPipelineComponentInvocation : IPipelineComponentInvocation
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

        public IRemoraOperation Operation { get; set; }

        public IPipelineComponentInvocation NextInvocation { get; set; }

        public IPipelineComponentInvocation PreviousInvocation { get; set; }

        public Action<IRemoraOperation> FinalCallback { get; set; }

        public virtual void BeginProcess()
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Calling NextInvocation[{0}].BeginProcess() on {1}...", NextInvocation, Operation);
            NextInvocation.BeginProcess();
        }

        public virtual void EndProcess()
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug("End of invocation chain, calling final callback...");
            FinalCallback.Invoke(Operation);
        }

        public override string ToString()
        {
            return "FinalCallback";
        }
    }
}
