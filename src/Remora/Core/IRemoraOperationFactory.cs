using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Remora.Core
{
    public interface IRemoraOperationFactory
    {
        IRemoraOperation Get(HttpRequest request);

        IRemoraOperation Get(HttpListenerRequest request);
    }
}
