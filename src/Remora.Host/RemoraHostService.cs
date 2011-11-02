using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Remora.Host.Configuration;

namespace Remora.Host
{
    public class RemoraHostService
    {
        private readonly IRemoraHostConfig _config;
        private static readonly ILog Log = LogManager.GetLogger(typeof(RemoraHostService));

        public RemoraHostService(IRemoraHostConfig config)
        {
            _config = config;
        }

        public void Start()
        {
            Log.InfoFormat("Starting {0} service...", _config.ServiceConfig.DisplayName);
            Log.InfoFormat("{0} service started.", _config.ServiceConfig.DisplayName);
        }

        public void Stop()
        {
            Log.InfoFormat("Stopping {0} service...", _config.ServiceConfig.DisplayName);
            Log.InfoFormat("{0} service stopped.", _config.ServiceConfig.DisplayName);
        }
    }
}
