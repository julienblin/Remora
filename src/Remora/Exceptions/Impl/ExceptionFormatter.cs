using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
using System.Web;
using Remora.Core;

namespace Remora.Exceptions.Impl
{
    public class ExceptionFormatter : IExceptionFormatter
    {
        #region IExceptionFormatter Members

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
                    WriteHtmlException(operation.Exception, response);
                    break;
            }
        }

        public virtual void WriteHtmlException(Exception exception, HttpResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.Write(string.Format(ErrorResources.GenericHtmlError, exception.GetType().Name.Replace("Exception", ""), exception.Message));
            response.Flush();
        }

        #endregion

        protected virtual void WriteSoap(IRemoraOperation operation, HttpResponse response)
        {
            response.ContentType = "text/xml";
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentEncoding = Encoding.UTF8;
            response.Write(string.Format(ErrorResources.SoapError, operation.Exception.GetType().Name.Replace("Exception", ""), operation.Exception.Message));
            response.Flush();
        }
    }
}
