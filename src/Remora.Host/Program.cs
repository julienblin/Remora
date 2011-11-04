#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Configuration;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using Remora.Host.Configuration;
using Topshelf;

namespace Remora.Host
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Program));

        private static int Main(string[] args)
        {
            ConfigureLog4Net();

            try
            {
                var config = RemoraHostConfigurationSectionHandler.GetConfiguration();

                HostFactory.Run(x =>
                                    {
                                        x.Service<RemoraHostService>(s =>
                                                                         {
                                                                             s.ConstructUsing(
                                                                                 name => new RemoraHostService(config));
                                                                             s.WhenStarted(rhs =>
                                                                                               {
                                                                                                   try
                                                                                                   {
                                                                                                       rhs.Start();
                                                                                                   }
                                                                                                   catch (Exception ex)
                                                                                                   {
                                                                                                       Log.Error(
                                                                                                           "There has been an error while starting the service.",
                                                                                                           ex);
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
                                                                                                       Log.Error(
                                                                                                           "There has been an error while stopping the service.",
                                                                                                           ex);
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
                var layout = new SimpleLayout();
                layout.ActivateOptions();
                var consoleAppender = new ConsoleAppender
                                          {
                                              Layout = layout
                                          };
                consoleAppender.AddFilter(new LevelRangeFilter
                                              {
                                                  LevelMin = Level.Info,
                                                  LevelMax = Level.Fatal,
                                                  AcceptOnMatch = false
                                              });
                consoleAppender.AddFilter(
                    new LoggerMatchFilter
                        {
                            LoggerToMatch = "Remora",
                            AcceptOnMatch = true
                        }
                    );
                consoleAppender.AddFilter(new DenyAllFilter());
                consoleAppender.ActivateOptions();
                BasicConfigurator.Configure(consoleAppender);
            }
            else
            {
                XmlConfigurator.Configure();
            }
        }
    }
}