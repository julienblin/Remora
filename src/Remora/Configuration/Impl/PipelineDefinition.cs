using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration.Impl
{
    public class PipelineDefinition : IPipelineDefinition
    {
        public PipelineDefinition()
        {
            ComponentDefinitions = new IComponentDefinition[0];
            Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public string Id { get; set; }

        public string UriFilterRegex { get; set; }

        public string UriRewriteRegex { get; set; }

        public IEnumerable<IComponentDefinition> ComponentDefinitions { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
