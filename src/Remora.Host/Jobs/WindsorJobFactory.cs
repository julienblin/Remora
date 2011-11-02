using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel;
using Castle.Windsor;
using Quartz;
using Quartz.Spi;

namespace Remora.Host.Jobs
{
    public class WindsorJobFactory : IJobFactory
    {
        private readonly IKernel _kernel;

        public WindsorJobFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public IJob NewJob(TriggerFiredBundle bundle)
        {
            return _kernel.Resolve<IJob>(bundle.JobDetail.Name);
        }
    }
}
