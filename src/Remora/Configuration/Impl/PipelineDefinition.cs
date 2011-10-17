using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration.Impl
{
    public class PipelineDefinition : IPipelineDefinition
    {
        public string Id { get; set; }

        public string UriFilterRegex { get; set; }

        public string UriRewriteRegex { get; set; }

        public IEnumerable<IComponentDefinition> ComponentDefinitions { get; set; }
    }
}
