using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration
{
    public interface IRemoraConfig
    {
        int MaxMessageSize { get; }

        IEnumerable<IPipelineDefinition> PipelineDefinitions { get; }

        IDictionary<string, string> Properties { get; }
    }
}
