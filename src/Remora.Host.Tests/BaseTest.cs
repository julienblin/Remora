using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Windsor;

namespace Remora.Host.Tests
{
    public abstract class BaseTest
    {
        private IWindsorContainer _container;

        protected ILogger GetConsoleLogger()
        {
            if (_container == null)
            {
                _container = new WindsorContainer();
                _container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.Console));
            }
            return _container.Resolve<ILogger>();
        }
    }
}
