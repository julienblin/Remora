using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration.Impl
{
    public class JobConfig : IJobConfig
    {
        public string Cron { get; set; }

        public string Name { get; set; }
    }
}
