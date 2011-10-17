using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Remora.Core;

namespace Remora.Exceptions.Impl
{
    public class ExceptionFormatter : IExceptionFormatter
    {
        public void WriteException(IRemoraOperation operation, HttpResponse response)
        {
            if (operation == null) throw new ArgumentNullException("operation");
            if (response == null) throw new ArgumentNullException("response");
            Contract.EndContractBlock();

            switch (operation.Kind)
            {
                case RemoraOperationKind.Soap:
                    WriteSoap(operation, response);
                    break;
                default:
                    WriteHtml(operation, response);
                    break;
            }
        }

        private void WriteSoap(IRemoraOperation operation, HttpResponse response)
        {
            response.ContentType = "text/xml";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentEncoding = Encoding.UTF8;
            response.Write(string.Format(ErrorResources.SoapError, operation.Exception.GetType().Name.Replace("Exception", ""), operation.Exception.Message));
            response.End();
        }

        protected virtual void WriteHtml(IRemoraOperation operation, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
