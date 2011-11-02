using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace Remora.Host.Jobs
{
    public class FakeJob : IJob
    {
        public void Execute(JobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
