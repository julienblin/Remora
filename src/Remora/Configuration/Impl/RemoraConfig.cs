using System;
using System.Collections.Generic;

namespace Remora.Configuration.Impl
{
    public class RemoraConfig : IRemoraConfig
    {
        public RemoraConfig()
        {
            PipelineDefinitions = new IPipelineDefinition[0];
            Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region IRemoraConfig Members

        public int MaxMessageSize { get; set; }

        public IEnumerable<IPipelineDefinition> PipelineDefinitions { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        #endregion
    }
}
