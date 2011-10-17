using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration
{
    public interface IRemoraConfig
    {
        int MaxMessagesSize { get; }

        IEnumerable<IPipelineDefinition> PipelineDefinitions { get; }
    }
}
