using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Windsor;

namespace Remora.Tests
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

        public Stream LoadSample(string name)
        {
            return typeof (BaseTest).Assembly.GetManifestResourceStream("Remora.Tests.Samples." + name);
        }
    }
}
