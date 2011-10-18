using System.Collections.Generic;

namespace Remora.Configuration
{
    public interface IRemoraConfig
    {
        int MaxMessageSize { get; }

        IEnumerable<IPipelineDefinition> PipelineDefinitions { get; }

        IDictionary<string, string> Properties { get; }
    }
}
