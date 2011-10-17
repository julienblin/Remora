using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Remora.Core.Impl
{
    public class RemoraRequest : IRemoraRequest
    {
        public RemoraRequest()
        {
            HttpHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public Uri Uri { get; set; }

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public string Method { get; set; }

        public byte[] Data { get; set; }
    }
}
