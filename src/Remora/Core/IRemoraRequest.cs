using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Remora.Core
{
    /// <summary>
    /// Abstract request
    /// </summary>
    public interface IRemoraRequest
    {
        string Uri { get; set; }

        IDictionary<string, string> HttpHeaders { get; }

        XDocument SoapHeaders { get; set; }

        XDocument SoapBody { get; set; }
    }
}