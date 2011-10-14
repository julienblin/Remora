using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora
{
    public class PipelineComponentDefinition
    {
        public PipelineComponentDefinition()
        {
            Properties = new Dictionary<string, string>();
        }

        public string Type { get; set; }

        public IDictionary<string, string> Properties { get; private set; }
    }
}
