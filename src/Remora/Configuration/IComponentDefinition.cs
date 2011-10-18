using System.Collections.Generic;

namespace Remora.Configuration
{
    public interface IComponentDefinition
    {
        string RefId { get; }

        IDictionary<string, string> Properties { get; }
    }
}
