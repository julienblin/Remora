using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Remora.Core.Impl
{
    public class RemoraResponse : IRemoraResponse
    {
        public RemoraResponse()
        {
            HttpHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public int StatusCode { get; set; }

        public byte[] Data { get; set; }
    }
}
