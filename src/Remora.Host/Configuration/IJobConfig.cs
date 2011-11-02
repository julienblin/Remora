using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration
{
    public interface IJobConfig
    {
        string Cron { get; }

        string Name { get; }
    }
}
