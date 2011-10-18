using System;
using System.Collections.Generic;

namespace Remora.Configuration.Impl
{
    public class ComponentDefinition : IComponentDefinition
    {
        public ComponentDefinition()
        {
            Properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region IComponentDefinition Members

        public string RefId { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        #endregion
    }
}
