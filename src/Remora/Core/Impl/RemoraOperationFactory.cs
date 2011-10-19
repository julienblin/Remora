#region License
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
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Configuration;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Core.Impl
{
    public class RemoraOperationFactory : IRemoraOperationFactory
    {
        public const string SoapEnvelopeSchema = @"http://schemas.xmlsoap.org/soap/envelope/";

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
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region IRemoraOperationFactory Members

        public IRemoraOperation Get(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Contract.EndContractBlock();

            var args = new InternalGetArgs
            {
                Uri = request.Url,
                Method = request.HttpMethod,
                ContentEncoding = request.ContentEncoding,
                Headers = request.Headers,
                InputStream = request.InputStream
            };

            return InternalGet(args);
        }

        public IRemoraOperation Get(HttpListenerRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Contract.EndContractBlock();

            var args = new InternalGetArgs
            {
                Uri = request.Url,
                Method = request.HttpMethod,
                ContentEncoding = request.ContentEncoding,
                Headers = request.Headers,
                InputStream = request.InputStream
            };

            return InternalGet(args);
        }

        #endregion

        public virtual IRemoraOperation InternalGet(InternalGetArgs args)
        {

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Creating IRemoraOperation for {0}...", args.Uri);

            IRemoraOperation operation;
            try
            {
                operation = _kernel.Resolve<IRemoraOperation>();
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException("Error while resolving IRemoraOperation from Windsor. Please make sure that the IRemoraOperation component is correctly registered.", ex);
            }

            operation.Request.Data = args.InputStream.ReadFully(_config.MaxMessageSize);
            operation.IncomingRequest.Data = operation.Request.Data;

            operation.IncomingRequest.Uri = args.Uri;
            operation.Request.Uri = args.Uri;

            operation.IncomingRequest.Method = args.Method;
            operation.Request.Method = args.Method;

            operation.IncomingRequest.ContentEncoding = args.ContentEncoding;
            operation.Request.ContentEncoding = args.ContentEncoding;

            foreach (string header in args.Headers)
            {
                operation.IncomingRequest.HttpHeaders.Add(header, args.Headers[header]);
                operation.Request.HttpHeaders.Add(header, args.Headers[header]);
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("IRemoraOperation {0} created.", operation);

            return operation;
        }

        public class InternalGetArgs
        {
            public Uri Uri { get; set; }

            public string Method { get; set; }

            public NameValueCollection Headers { get; set; }

            public Stream InputStream { get; set; }

            public Encoding ContentEncoding { get; set; }
        }
    }
}
