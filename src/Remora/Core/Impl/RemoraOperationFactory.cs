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
using System.Diagnostics.Contracts;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Configuration;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Core.Impl
{
    public class RemoraOperationFactory : IRemoraOperationFactory
    {
        public const string RequestContextKey = @"Request";

        private readonly IRemoraConfig _config;
        private readonly IKernel _kernel;
        private ILogger _logger = NullLogger.Instance;

        public RemoraOperationFactory(IKernel kernel, IRemoraConfig config)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (config == null) throw new ArgumentNullException("config");
            Contract.EndContractBlock();

            _kernel = kernel;
            _config = config;
        }

        /// <summary>
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IRemoraOperationFactory Members

        public IRemoraOperation Get(IUniversalRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Creating IRemoraOperation for {0}...", request.Url);

            IRemoraOperation operation;
            try
            {
                operation = _kernel.Resolve<IRemoraOperation>();
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException(
                    "Error while resolving IRemoraOperation from Windsor. Please make sure that the IRemoraOperation component is correctly registered.",
                    ex);
            }

            operation.Request.Data = request.InputStream.ReadFully(_config.MaxMessageSize);

            operation.IncomingUri = request.Url;
            operation.Request.Uri = request.Url;
            operation.Request.ContentEncoding = request.ContentEncoding;
            operation.Request.Method = request.HttpMethod;

            foreach (var header in request.Headers.Keys)
            {
                operation.Request.HttpHeaders.Add(header, request.Headers[header]);
            }

            operation.ExecutionProperties[RequestContextKey] = request;

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("IRemoraOperation {0} created.", operation);

            return operation;
        }

        #endregion
    }
}