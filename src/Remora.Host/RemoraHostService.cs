using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using log4net;
using Remora.Core;
using Remora.Exceptions;
using Remora.Handler;
using Remora.Host.Configuration;
using Remora.Host.Exceptions;
using Remora.Pipeline;

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

            Log.Debug("Bootstrapping Remora...");
            Bootstraper.Init();

            Log.DebugFormat("Configuring {0} to listen to prefixes: {1}",
                 _config.ServiceConfig.DisplayName,
                 string.Join(",", _config.ListenerConfigs.Select(x => x.Prefix))
            );
            _httpListener = new HttpListener();
            foreach (var listenerConfig in _config.ListenerConfigs)
            {
                _httpListener.Prefixes.Add(listenerConfig.Prefix);
            }

            Log.Debug("Starting HttpListener...");
            _httpListener.Start();

            _httpListener.BeginGetContext(ListenerCallback, _httpListener);

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

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var listener = (HttpListener)result.AsyncState;
                var context = listener.EndGetContext(result);
                
                Log.DebugFormat("Begin async process for request coming from {0}...", context.Request.Url);

                var operationFactory = Bootstraper.Container.Resolve<IRemoraOperationFactory>();
                var kindIdentifier = Bootstraper.Container.Resolve<IRemoraOperationKindIdentifier>();
                var pipelineFactory = Bootstraper.Container.Resolve<IPipelineFactory>();
                var pipelineEngine = Bootstraper.Container.Resolve<IPipelineEngine>();

                var operation = operationFactory.Get(context.Request);
                operation.Kind = kindIdentifier.Identify(operation);
                var pipeline = pipelineFactory.Get(operation);

                if (pipeline == null)
                    throw new InvalidConfigurationException(
                        string.Format("Unable to select an appropriate pipeline for operation {0}.", operation));

                operation.ExecutionProperties["HttpListener.Response"] = context.Response;

                pipelineEngine.RunAsync(operation, pipeline, EngineCallback);
            }
            catch (Exception ex)
            {
                Log.Error("There has been an error while handling a request.", ex);
                throw;
            }
        }

        private void EngineCallback(IRemoraOperation operation)
        {
            if (Log.IsDebugEnabled)
                Log.DebugFormat("Async process ended for request coming from {0}. Writing results...", operation.IncomingUri);

            var response = (HttpListenerResponse)operation.ExecutionProperties["HttpListener.Response"];

            var writer = Bootstraper.Container.Resolve<IResponseWriter>();
            writer.Write(operation, response);

            response.OutputStream.Close();
        }
    }
}
