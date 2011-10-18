using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Web;
using Castle.Core.Logging;
using Remora.Core;
using Remora.Exceptions;

namespace Remora.Handler.Impl
{
    public class ResponseWriter : IResponseWriter
    {
        private readonly IExceptionFormatter _exceptionformatter;

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public ResponseWriter(IExceptionFormatter exceptionformatter)
        {
            if(exceptionformatter == null) throw new ArgumentNullException("exceptionFormatter");
            Contract.EndContractBlock();

            _exceptionformatter = exceptionformatter;
        }

        public void Write(IRemoraOperation operation, HttpResponse response)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            if (operation.OnError)
            {
                Logger.ErrorFormat(operation.Exception, "There has been an error when processing request coming from {0}.", operation.IncomingRequest.Uri);
                _exceptionformatter.WriteException(operation, response);
            }
            else
            {
                response.StatusCode = operation.Response.StatusCode;
                foreach (var header in operation.Response.HttpHeaders)
                {
                    response.AppendHeader(header.Key, header.Value);
                }

                if (operation.Response.Data != null)
                {
                    response.OutputStream.Write(operation.Response.Data, 0, operation.Response.Data.Length);
                }
                response.OutputStream.Flush();
            }
        }
    }
}
