using System.Collections.Generic;

namespace Remora.Configuration
{
    public interface IPipelineDefinition
    {
        string Id { get; }

        string UriFilterRegex { get; }

        string UriRewriteRegex { get; }

        IEnumerable<IComponentDefinition> ComponentDefinitions { get; }

        IDictionary<string, string> Properties { get; }
    }
}
