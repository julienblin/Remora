using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Remora.Host.Configuration;
using Remora.Host.Exceptions;

namespace Remora.Host
{
    public class RemoraHostService
    {
        private readonly IRemoraHostConfig _config;
        private static readonly ILog Log = LogManager.GetLogger(typeof(RemoraHostService));

        private HttpListener _httpListener;
        private Thread _listenerThread;
        private ManualResetEvent _stop;

        public RemoraHostService(IRemoraHostConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            if (_config.BindingConfigs.Count() == 0)
                throw new RemoraHostServiceException(string.Format("Unable to start {0}: no prefixes has been defined.", _config.ServiceConfig.DisplayName));

            Log.InfoFormat("Starting {0}...", _config.ServiceConfig.DisplayName);

            _stop = new ManualResetEvent(false);
            _listenerThread = new Thread(HandleRequests);

            Log.Debug("Bootstrapping Remora...");
            Bootstraper.Init();

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

            Log.InfoFormat("{0} started.", _config.ServiceConfig.DisplayName);
        }

        public void Stop()
        {
            Log.InfoFormat("Stopping {0}...", _config.ServiceConfig.DisplayName);

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

            Log.InfoFormat("{0} stopped.", _config.ServiceConfig.DisplayName);
        }

        private void HandleRequests()
        {
            while (_httpListener.IsListening)
            {
                var context = _httpListener.BeginGetContext(ListenerCallback, null);

                if (0 == WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }))
                    return;
            }
        }

        private void ListenerCallback(IAsyncResult result)
        {
            Task.Factory.StartNew(() =>
            {
                var context = _httpListener.EndGetContext(result);

                var remoraAsyncResult = new RemoraAsyncResult(RemoraAsyncResultCallback, context, null, Bootstraper.Container);
                remoraAsyncResult.Process();
            });
        }

        private static void RemoraAsyncResultCallback(IAsyncResult result)
        {
            var remoraAsyncResult = (RemoraAsyncResult) result;
            remoraAsyncResult.HttpListenerContext.Response.OutputStream.Close();
        }
    }
}
