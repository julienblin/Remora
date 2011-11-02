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
            BindingConfigs = new IBindingConfig[0];
            JobsConfig = new JobsConfig();
        }

        public IServiceConfig ServiceConfig { get; set; }

        public IEnumerable<IBindingConfig> BindingConfigs { get; set; }

        public IJobsConfig JobsConfig { get; set; }
    }
}
