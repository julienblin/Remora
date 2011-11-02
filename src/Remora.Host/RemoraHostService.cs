using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        public RemoraHostService(IRemoraHostConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            if (_config.ListenerConfigs.Count() == 0)
                throw new RemoraHostServiceException(string.Format("Unable to start {0}: no prefixes has been defined.", _config.ServiceConfig.DisplayName));

            Log.InfoFormat("Starting {0}...", _config.ServiceConfig.DisplayName);

            Log.DebugFormat("Configuring {0} to listen to prefixes: {1}",
                 _config.ServiceConfig.DisplayName,
                 string.Join(",", _config.ListenerConfigs.Select(x => x.Prefix))
            );
            _httpListener = new HttpListener();
            foreach (var listenerConfig in _config.ListenerConfigs)
            {
                _httpListener.Prefixes.Add(listenerConfig.Prefix);
            }
            _httpListener.Start();

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
    }
}
