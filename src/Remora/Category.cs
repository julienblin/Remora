using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora
{
    public class Category
    {
        public Category()
        {
            PipelineComponents = new List<PipelineComponentDefinition>();
        }

        public string Name { get; set; }

        public string UrlMatcher { get; set; }

        public ICollection<PipelineComponentDefinition> PipelineComponents { get; private set; }
    }
}
