using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Configuration
{
    public interface IComponentDefinition
    {
        string RefId { get; }

        IDictionary<string, string> Properties { get; }
    }
}
