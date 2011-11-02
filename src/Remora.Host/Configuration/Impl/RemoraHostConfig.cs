using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration.Impl
{
    public class RemoraHostConfig : IRemoraHostConfig
    {
        public RemoraHostConfig()
        {
            ServiceConfig = new ServiceConfig();
            ListenerConfigs = new IListenerConfig[0];
        }

        public IServiceConfig ServiceConfig { get; set; }

        public IEnumerable<IListenerConfig> ListenerConfigs { get; set; }
    }
}
