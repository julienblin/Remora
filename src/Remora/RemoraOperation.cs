using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Remora
{
    public class RemoraOperation
    {
        public HttpContext HttpContext { get; set; }

        public XDocument SoapRequest { get; set; }

        public XDocument SoapResponse { get; set; }

        public string DestinationUrl { get; set; }
    }
}
