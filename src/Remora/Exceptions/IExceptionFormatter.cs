using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Remora.Core;

namespace Remora.Exceptions
{
    public interface IExceptionFormatter
    {
        void WriteException(IRemoraOperation operation, HttpResponse response);
    }
}
