using System;
using System.Collections.Generic;

namespace Remora.Core.Impl
{
    public class RemoraRequest : IRemoraRequest
    {
        public RemoraRequest()
        {
            HttpHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region IRemoraRequest Members

        public Uri Uri { get; set; }

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public string Method { get; set; }

        public byte[] Data { get; set; }

        #endregion
    }
}
