using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration
{
    public interface IRemoraHostConfig
    {
        string ServiceName { get; }

        string DisplayName { get; }

        string Description { get; }

        IEnumerable<IListenerConfig> ListenerConfigs { get; }
    }
}
