using System.Net;
using System.Web;

namespace Remora.Core
{
    public interface IRemoraOperationFactory
    {
        IRemoraOperation Get(HttpRequest request);

        IRemoraOperation Get(HttpListenerRequest request);
    }
}
