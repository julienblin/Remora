using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Remora.Core
{
    public interface IUniversalRequest
    {
        IEnumerable<string> AcceptTypes { get; }

        Encoding ContentEncoding { get; }

        long ContentLength { get; }

        string ContentType { get; }

        IDictionary<string, string> Cookies { get; }

        IDictionary<string, string> Headers { get; }

        string HttpMethod { get; }

        Stream InputStream { get; }

        bool IsAuthenticated { get; }

        bool IsLocal { get; }

        bool IsSecureConnection { get; }

        IDictionary<string, string> QueryString { get; }

        string RawUrl { get; }

        Uri Url { get; }

        Uri UrlReferrer { get; }

        string UserAgent { get; }

        string UserHostAddress { get; }

        string UserHostName { get; }

        IEnumerable<string> UserLanguages { get; }

        object OriginalRequest { get; }
    }
}
