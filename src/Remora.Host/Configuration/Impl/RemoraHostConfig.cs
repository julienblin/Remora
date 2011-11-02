using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration.Impl
{
    public class RemoraHostConfig : IRemoraHostConfig
    {
        public static class Defaults
        {
            public const string ServiceName = "remorahost";
            public const string DisplayName = "Remora Host";
            public const string Description = "Remora host service";
        }

        public RemoraHostConfig()
        {
            ServiceName = Defaults.ServiceName;
            DisplayName = Defaults.DisplayName;
            Description = Defaults.Description;
            ListenerConfigs = new IListenerConfig[0];
        }

        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public IEnumerable<IListenerConfig> ListenerConfigs { get; set; }
    }
}
