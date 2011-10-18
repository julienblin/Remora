using System;
using Remora.Core;

namespace Remora.Pipeline
{
    public abstract class AbstractPipelineComponent : IPipelineComponent
    {
        #region IPipelineComponent Members

        public virtual void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
        {
            callback(true);
        }

        public virtual void EndAsyncProcess(IRemoraOperation operation, Action callback)
        {
            callback();
        }

        #endregion
    }
}
