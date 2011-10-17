using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration.Impl
{
    public class ComponentDefinition : IComponentDefinition
    {
        public ComponentDefinition()
        {
            Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public string RefId { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
