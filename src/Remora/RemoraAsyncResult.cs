using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Castle.Core.Logging;
using Castle.Windsor;
using Remora.Core;
using Remora.Pipeline;

namespace Remora
{
    public class RemoraAsyncResult : IAsyncResult
    {
        private readonly AsyncCallback _callback;
        private readonly IWindsorContainer _container;
        private ILogger _logger = NullLogger.Instance;

        public RemoraAsyncResult(AsyncCallback cb, HttpContext context, object state, IWindsorContainer container)
        {
            _callback = cb;
            _container = container;
            Context = context;
            AsyncState = state;
        }

        public bool IsCompleted { get; private set; }
        public WaitHandle AsyncWaitHandle { get { return null; } }
        public object AsyncState { get; private set; }
        public bool CompletedSynchronously { get { return false; } }
        public HttpContext Context { get; private set; }

        public void Process()
        {
            try
            {
                _logger = _container.Resolve<ILogger>();
            }
            catch { }

            try
            {
                if (_logger.IsDebugEnabled)
                    _logger.DebugFormat("Begin async process for request coming from {0}...", Context.Request.Url);

                var operationFactory = _container.Resolve<IRemoraOperationFactory>();
                var pipelineFactory = _container.Resolve<IPipelineFactory>();
                var pipelineEngine = _container.Resolve<IPipelineEngine>();

                var operation = operationFactory.Get(Context.Request);
                var pipeline = pipelineFactory.Get(operation);

                pipelineEngine.RunAsync(operation, pipeline, EngineCallback);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "There has been an error when processing request coming from {0}.", Context.Request.Url);

                WriteGenericException(ex);
                IsCompleted = true;
                _callback(this);
            }
        }

        public void EngineCallback(IRemoraOperation operation)
        {
            if (_logger.IsDebugEnabled)
                _logger.DebugFormat("Async process ended for request coming from {0}. Writing results...", Context.Request.Url);

            if (operation.OnError)
            {
                _logger.ErrorFormat(operation.Exception, "There has been an error when processing request coming from {0}.", Context.Request.Url);
                WriteOperationException(operation);
            }
            else
            {
                Context.Response.StatusCode = operation.Response.StatusCode;
                foreach (var header in operation.Response.HttpHeaders)
                {
                    Context.Response.AppendHeader(header.Key, header.Value);
                }

                if (operation.Response.Data != null)
                {
                    Context.Response.OutputStream.Write(operation.Response.Data, 0, operation.Response.Data.Length);
                }
            }

            IsCompleted = true;
            _callback(this);
        }

        private void WriteGenericException(Exception exception)
        {
            Context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            Context.Response.ContentType = "text/html";
            Context.Response.ContentEncoding = Encoding.UTF8;
            Context.Response.Write(string.Format(ErrorResources.GenericHtmlError, exception.Message));
            Context.Response.End();
        }

        private void WriteOperationException(IRemoraOperation operation)
        {
            WriteGenericException(operation.Exception);
        }
    }
}
