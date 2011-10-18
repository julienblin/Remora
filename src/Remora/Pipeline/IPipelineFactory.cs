using Remora.Core;

namespace Remora.Pipeline
{
    public interface IPipelineFactory
    {
        IPipeline Get(IRemoraOperation operation);
    }
}
