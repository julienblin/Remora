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
                    x.Service<RemoraHostService>(s =>
                    {
                        s.ConstructUsing(name => new RemoraHostService(config));
                        s.WhenStarted(rhs =>
                        {
                            try
                            {
                                rhs.Start();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("There has been an error while starting the service.", ex);
                                throw;
                            }
                        });
                        s.WhenStopped(rhs =>
                        {
                            try
                            {
                                rhs.Stop();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("There has been an error while stopping the service.", ex);
                            }
                        });
                    });

                    switch (config.ServiceConfig.RunAs)
                    {
                        case ServiceConfigRunAs.LocalService:
                            x.RunAsLocalService();
                            break;
                        case ServiceConfigRunAs.LocalSystem:
                            x.RunAsLocalSystem();
                            break;
                        case ServiceConfigRunAs.NetworkService:
                            x.RunAsNetworkService();
                            break;
                        case ServiceConfigRunAs.User:
                            x.RunAs(config.ServiceConfig.Username, config.ServiceConfig.Password);
                            break;
                    }

                    x.SetDescription(config.ServiceConfig.Description);
                    x.SetDisplayName(config.ServiceConfig.DisplayName);
                    x.SetServiceName(config.ServiceConfig.ServiceName);
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
                var consoleAppender = new log4net.Appender.ConsoleAppender
                {
                    Layout = layout
                };
                consoleAppender.AddFilter(
                    new log4net.Filter.LoggerMatchFilter
                        {
                            LoggerToMatch = "Remora",
                            AcceptOnMatch = true,
                            Next = new log4net.Filter.LevelRangeFilter { LevelMin = Level.Info, AcceptOnMatch = true }
                        }
                );
                consoleAppender.AddFilter(new log4net.Filter.DenyAllFilter());
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
