﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Core.Impl
{
    public class RemoraOperationFactory : IRemoraOperationFactory
    {
        public const string SoapEnvelopeSchema = @"http://schemas.xmlsoap.org/soap/envelope/";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private readonly IKernel _kernel;

        public RemoraOperationFactory(IKernel kernel)
        {
            if(kernel == null) throw new ArgumentNullException("kernel");
            Contract.EndContractBlock();

            _kernel = kernel;
        }

        public IRemoraOperation Get(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Contract.EndContractBlock();

            return InternalGet(request.Url, request.Headers, request.InputStream);
        }

        public IRemoraOperation Get(HttpListenerRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");
            Contract.EndContractBlock();

            return InternalGet(request.Url, request.Headers, request.InputStream);
        }

        public virtual IRemoraOperation InternalGet(Uri uri, NameValueCollection headers, Stream inputStream)
        {
            IRemoraOperation operation;
            try
            {
                operation = _kernel.Resolve<IRemoraOperation>();
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException("Error while resolving IRemoraOperation from Windsor. Please make sure that the IRemoraOperation component is correctly registered.", ex);
            }

            operation.IncomingUri = uri;

            foreach (string header in headers)
            {
                operation.Request.HttpHeaders.Add(header, headers[header]);
            }

            operation.Request.Data = inputStream.ReadFully();

            return operation;
        }
    }
}