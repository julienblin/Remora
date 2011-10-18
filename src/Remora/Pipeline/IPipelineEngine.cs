using System;
using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineEngine
    {
        void RunAsync(IRemoraOperation operation, IPipeline pipeline, Action<IRemoraOperation> callback);
    }
}
