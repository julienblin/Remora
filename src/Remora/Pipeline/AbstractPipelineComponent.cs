using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    public abstract class AbstractPipelineComponent : IPipelineComponent
    {
        public virtual void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
        {
            callback(true);
        }

        public virtual void EndAsyncProcess(IRemoraOperation operation, Action callback)
        {
            callback();
        }
    }
}
