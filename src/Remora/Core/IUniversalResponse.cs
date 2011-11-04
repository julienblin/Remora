using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Remora.Core
{
    public interface IUniversalResponse
    {
        Encoding ContentEncoding { get; set; }

        string ContentType { get; set; }

        IDictionary<string, string> Headers { get; }

        void SetHeader(string name, string value);

        Stream OutputStream { get; }

        string RedirectLocation { get; set; }

        int StatusCode { get; set; }

        string StatusDescription { get; set; }

        object OriginalResponse { get; }
    }
}
