using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Remora.Core.Impl
{
    public class RemoraResponse : IRemoraResponse
    {
        public RemoraResponse()
        {
            HttpHeaders = new Dictionary<string, string>();
        }

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public XDocument SoapHeaders { get; set; }

        public XDocument SoapBody { get; set; }
    }
}
