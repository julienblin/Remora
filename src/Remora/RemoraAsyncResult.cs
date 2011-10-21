﻿#region Licence

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
using System.Threading;
using System.Web;
using Castle.Core.Logging;
using Castle.Windsor;
using Remora.Core;
using Remora.Exceptions;
using Remora.Exceptions.Impl;
using Remora.Handler;
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

        public HttpContext Context { get; private set; }

        #region IAsyncResult Members

        public bool IsCompleted { get; private set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public object AsyncState { get; private set; }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        #endregion

        public void Process()
        {
            try
            {
                _logger = _container.Resolve<ILogger>();
            }
            catch
            {
            }

            try
            {
                if (_logger.IsDebugEnabled)
                    _logger.DebugFormat("Begin async process for request coming from {0}...", Context.Request.Url);

                var operationFactory = _container.Resolve<IRemoraOperationFactory>();
                var kindIdentifier = _container.Resolve<IRemoraOperationKindIdentifier>();
                var pipelineFactory = _container.Resolve<IPipelineFactory>();
                var pipelineEngine = _container.Resolve<IPipelineEngine>();

                var operation = operationFactory.Get(Context.Request);
                operation.Kind = kindIdentifier.Identify(operation);
                var pipeline = pipelineFactory.Get(operation);

                if (pipeline == null)
                    throw new InvalidConfigurationException(
                        string.Format("Unable to select an appropriate pipeline for operation {0}.", operation));

                pipelineEngine.RunAsync(operation, pipeline, EngineCallback);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat(ex, "There has been an error when processing request coming from {0}.",
                                    Context.Request.Url);

                WriteGenericException(ex);
                IsCompleted = true;
                _callback(this);
            }
        }

        public void EngineCallback(IRemoraOperation operation)
        {
            if (_logger.IsDebugEnabled)
                _logger.DebugFormat("Async process ended for request coming from {0}. Writing results...",
                                    Context.Request.Url);

            var writer = _container.Resolve<IResponseWriter>();
            writer.Write(operation, Context.Response);

            IsCompleted = true;
            _callback(this);
        }

        private void WriteGenericException(Exception exception)
        {
            IExceptionFormatter formatter;
            try
            {
                formatter = _container.Resolve<IExceptionFormatter>();
            }
            catch
            {
                formatter = new ExceptionFormatter();
            }
            formatter.WriteHtmlException(exception, Context.Response);
        }
    }
}