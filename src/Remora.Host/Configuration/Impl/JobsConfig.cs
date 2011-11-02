using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration.Impl
{
    public class JobsConfig : IJobsConfig
    {
        public JobsConfig()
        {
            JobConfigs = new JobConfig[0];
        }

        public IEnumerable<IJobConfig> JobConfigs { get; set; }
    }
}
