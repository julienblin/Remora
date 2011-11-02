using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration.Impl
{
    public class ServiceConfig : IServiceConfig
    {
        public static class Defaults
        {
            public const string ServiceName = "remorahost";
            public const string DisplayName = "Remora Host";
            public const string Description = "Remora host service";
            public const ServiceConfigRunAs RunAs = ServiceConfigRunAs.LocalService;
        }


        public ServiceConfig()
        {
            ServiceName = Defaults.ServiceName;
            DisplayName = Defaults.DisplayName;
            Description = Defaults.Description;
            RunAs = Defaults.RunAs;
        }

        public string ServiceName { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ServiceConfigRunAs RunAs { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
