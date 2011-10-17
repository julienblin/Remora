using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
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
        private readonly IRemoraConfig _config;

        public RemoraOperationFactory(IKernel kernel, IRemoraConfig config)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");
            if (config == null) throw new ArgumentNullException("config");
            Contract.EndContractBlock();

            _kernel = kernel;
            _config = config;
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

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Creating IRemoraOperation for {0}...", uri);

            IRemoraOperation operation;
            try
            {
                operation = _kernel.Resolve<IRemoraOperation>();
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationException("Error while resolving IRemoraOperation from Windsor. Please make sure that the IRemoraOperation component is correctly registered.", ex);
            }

            operation.Request.Data = inputStream.ReadFully(_config.MaxMessageSize);

            operation.IncomingRequest.Uri = uri;
            operation.Request.Uri = uri;

            foreach (string header in headers)
            {
                operation.IncomingRequest.HttpHeaders.Add(header, headers[header]);
                operation.Request.HttpHeaders.Add(header, headers[header]);
            }

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("IRemoraOperation {0} created.", operation);

            return operation;
        }
    }
}
