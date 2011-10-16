using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineEngine
    {
        void RunAsync(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback);
    }
}
