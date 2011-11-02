using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;
using Remora.Host.Configuration;
using Topshelf;

namespace Remora.Host
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static int Main(string[] args)
        {
            ConfigureLog4Net();

            try
            {
                var config = RemoraHostConfigurationSectionHandler.GetConfiguration();

                HostFactory.Run(x =>
                {
                    //x.Service<TownCrier>(s => 
                    //{
                    //    s.ConstructUsing(name => new TownCrier());
                    //    s.WhenStarted(tc => tc.Start());
                    //    s.WhenStopped(tc => tc.Stop());
                    //});
                    //x.RunAsLocalSystem();

                    x.SetDescription(config.Description);
                    x.SetDisplayName(config.DisplayName);
                    x.SetServiceName(config.ServiceName);
                });

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal("There has been an error while starting Remora host.", ex);
                return -1;
            }
        }

        private static void ConfigureLog4Net()
        {
            if (ConfigurationManager.GetSection("log4net") == null)
            {
                var layout = new log4net.Layout.SimpleLayout();
                layout.ActivateOptions();
                var consoleAppender = new log4net.Appender.ColoredConsoleAppender
                {
                    Layout = layout
                };
                consoleAppender.AddFilter(new log4net.Filter.LevelRangeFilter { LevelMin = Level.Info, AcceptOnMatch = true });
                consoleAppender.ActivateOptions();
                log4net.Config.BasicConfigurator.Configure(consoleAppender);
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
            }
        }
    }
}
