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
            Properties = new Dictionary<string, string>();
            PipelineComponents = new List<PipelineComponentDefinition>();
        }

        public string Name { get; set; }

        public IDictionary<string, string> Properties { get; private set; }

        public ICollection<PipelineComponentDefinition> PipelineComponents { get; private set; }
    }
}
