using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration.Impl
{
    public class RemoraConfig : IRemoraConfig
    {
        public RemoraConfig()
        {
            PipelineDefinitions = new IPipelineDefinition[0];
        }

        public int MaxMessagesSize { get; set; }

        public IEnumerable<IPipelineDefinition> PipelineDefinitions { get; set; }
    }
}
