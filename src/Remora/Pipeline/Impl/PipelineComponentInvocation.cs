using System;
using Castle.Core.Logging;
using Remora.Core;

namespace Remora.Pipeline.Impl
{
    public class PipelineComponentInvocation : IPipelineComponentInvocation
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

        public IPipelineComponent Component { get; set; }

        #region IPipelineComponentInvocation Members

        public IRemoraOperation Operation { get; set; }

        public IPipelineComponentInvocation NextInvocation { get; set; }

        public IPipelineComponentInvocation PreviousInvocation { get; set; }

        public virtual void BeginProcess()
        {
            try
            {
                if(Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling Component[{0}].BeginAsyncProcess({1})...", Component, Operation);
                Component.BeginAsyncProcess(Operation, BeginProcessCallback);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error while calling Component[{0}].BeginAsyncProcess({1}).", Component, Operation);
                Operation.Exception = ex;

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling BeginProcessCallback(false)...", Component, Operation);
                BeginProcessCallback(false);
            }
        }

        public virtual void EndProcess()
        {
            try
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling Component[{0}].EndAsyncProcess({1})...", Component, Operation);
                Component.EndAsyncProcess(Operation, PreviousInvocation.EndProcess);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error while calling Component[{0}].EndAsyncProcess({1}).", Component, Operation);
                Operation.Exception = ex;

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling PreviousInvocation[{0}].EndProcess() on {1}...", PreviousInvocation, Operation);
                PreviousInvocation.EndProcess();
            }
        }

        #endregion

        public virtual void BeginProcessCallback(bool continueProcess)
        {
            if(continueProcess)
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling NextInvocation[{0}].BeginProcess() on {1}...", NextInvocation, Operation);
                NextInvocation.BeginProcess();
            }
            else
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Calling PreviousInvocation[{0}].EndProcess() on {1}...", PreviousInvocation, Operation);
                PreviousInvocation.EndProcess();
            }
        }

        public override string ToString()
        {
            return Component.GetType().Name;
        }
    }
}
