using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration.Impl
{
    public class RemoraConfig : IRemoraConfig
    {
        public int MaxMessagesSize { get; set; }

        public IEnumerable<IPipelineDefinition> PipelineDefinitions { get; set; }
    }
}
