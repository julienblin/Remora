using System;
using System.Web;
using Remora.Core;

namespace Remora.Exceptions
{
    public interface IExceptionFormatter
    {
        void WriteException(IRemoraOperation operation, HttpResponse response);

        void WriteHtmlException(Exception exception, HttpResponse response);
    }
}
