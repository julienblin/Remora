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
            HttpHeaders = new Dictionary<string, string>();
        }

        public string Uri { get; set; }

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public XDocument SoapPayload { get; set; }

        public XElement SoapHeaders { get; set; }

        public XElement SoapBody { get; set; }
    }
}
