using System.Web;
using Remora.Core;

namespace Remora.Handler
{
    public interface IResponseWriter
    {
        void Write(IRemoraOperation operation, HttpResponse response);
    }
}
