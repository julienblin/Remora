using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineEngine
    {
        void Run(IRemoraOperation operation, IPipeline pipeline);
    }
}
