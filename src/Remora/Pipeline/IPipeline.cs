using System.Collections.Generic;

namespace Remora.Pipeline
{
    public interface IPipeline
    {
        string Id { get; }

        IEnumerable<IPipelineComponent> Components { get; }
    }
}
