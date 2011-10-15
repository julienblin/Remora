using System.Collections.Generic;
using System.Xml.Linq;

namespace Remora.Core
{
    /// <summary>
    /// Abstract response
    /// </summary>
    public interface IRemoraResponse
    {
        IDictionary<string, string> HttpHeaders { get; }

        XDocument SoapHeaders { get; set; }

        XDocument SoapBody { get; set; }
    }
}