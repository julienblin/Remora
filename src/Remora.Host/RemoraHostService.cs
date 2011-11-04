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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Common.Logging.Log4Net;
using log4net;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Remora.Host.Configuration;
using Remora.Host.Exceptions;
using Remora.Host.Jobs;

namespace Remora.Host
{
    public class RemoraHostService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (RemoraHostService));
        private readonly IRemoraHostConfig _config;

        private HttpListener _httpListener;
        private Thread _listenerThread;

        private IScheduler _scheduler;
        private ManualResetEvent _stop;

        public RemoraHostService(IRemoraHostConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            Log.InfoFormat("Starting {0}...", _config.ServiceConfig.DisplayName);

            Log.Debug("Bootstrapping Remora...");
            Bootstraper.Init();

            StartHttpListener();
            StartJobs();

            Log.InfoFormat("{0} started.", _config.ServiceConfig.DisplayName);
        }

        private void StartHttpListener()
        {
            if (_config.BindingConfigs.Count() == 0)
                throw new RemoraHostServiceException(string.Format(
                    "Unable to start {0}: no prefixes has been defined.", _config.ServiceConfig.DisplayName));

            _stop = new ManualResetEvent(false);
            _listenerThread = new Thread(HandleRequests);

            Log.DebugFormat("Configuring {0} to listen to prefixes: {1}",
                            _config.ServiceConfig.DisplayName,
                            string.Join(",", _config.BindingConfigs.Select(x => x.Prefix))
                );
            _httpListener = new HttpListener();
            foreach (var listenerConfig in _config.BindingConfigs)
            {
                _httpListener.Prefixes.Add(listenerConfig.Prefix);
            }

            Log.Debug("Starting HttpListener...");
            _httpListener.Start();
            _listenerThread.Start();
        }

        private void StartJobs()
        {
            if (_config.JobsConfig.JobConfigs.Count() > 0)
            {
                var commonLoggingConfig = new NameValueCollection
                                              {
                                                  {"configType", "EXTERNAL"}
                                              };
                Common.Logging.LogManager.Adapter = new Log4NetLoggerFactoryAdapter(commonLoggingConfig);

                Bootstraper.Container.Register(
                    Component.For<IJobFactory>().ImplementedBy<WindsorJobFactory>(),
                    Component.For<ISchedulerFactory>().ImplementedBy<StdSchedulerFactory>(),
                    Component.For<IScheduler>().UsingFactoryMethod((k, c) =>
                                                                       {
                                                                           var sched =
                                                                               k.Resolve<ISchedulerFactory>().
                                                                                   GetScheduler();
                                                                           sched.JobFactory = k.Resolve<IJobFactory>();
                                                                           return sched;
                                                                       })
                    );

                Log.Debug("Starting JobScheduler...");
                _scheduler = Bootstraper.Container.Resolve<IScheduler>();
                _scheduler.Start();

                foreach (var jobConfig in _config.JobsConfig.JobConfigs)
                {
                    Log.DebugFormat("Scheduling job {0} @ {1}...", jobConfig.Name, jobConfig.Cron);
                    var jobDetail = new JobDetail(jobConfig.Name, typeof (FakeJob));
                    var trigger = new CronTrigger("cronTrigger", "default", jobConfig.Cron);
                    _scheduler.ScheduleJob(jobDetail, trigger);
                }
            }
        }

        public void Stop()
        {
            Log.InfoFormat("Stopping {0}...", _config.ServiceConfig.DisplayName);

            StopHttpListener();
            StopJobs();

            Log.InfoFormat("{0} stopped.", _config.ServiceConfig.DisplayName);
        }

        private void StopJobs()
        {
            if (_scheduler != null)
            {
                _scheduler.Shutdown(true);
            }
        }

        private void StopHttpListener()
        {
            if (_httpListener != null)
            {
                try
                {
                    Log.Debug("Stopping HttpListener...");
                    _stop.Set();
                    _listenerThread.Join();
                    _httpListener.Stop();
                    Log.Debug("HttpListener stopped.");
                }
                catch (Exception ex)
                {
                    Log.Fatal("An error occured while stopping http listener.", ex);
                }
            }
        }

        private void HandleRequests()
        {
            while (_httpListener.IsListening)
            {
                var context = _httpListener.BeginGetContext(ListenerCallback, null);

                if (0 == WaitHandle.WaitAny(new[] {_stop, context.AsyncWaitHandle}))
                    return;
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          try
                                          {
                                              var context = _httpListener.EndGetContext(result);

                                              var remoraAsyncResult = new RemoraAsyncProcessor(
                                                  RemoraAsyncResultCallback, context, null, Bootstraper.Container);
                                              remoraAsyncResult.Process();
                                          }
                                          catch (Exception)
                                          {
                                              if (_httpListener.IsListening)
                                                  throw;
                                          }
                                      });
        }

        private static void RemoraAsyncResultCallback(IAsyncResult result)
        {
            var remoraAsyncResult = (RemoraAsyncProcessor) result;
            remoraAsyncResult.HttpListenerContext.Response.OutputStream.Close();
        }
    }
}