using System;
using System.Collections.Generic;

namespace Remora.Core.Impl
{
    public class RemoraResponse : IRemoraResponse
    {
        public RemoraResponse()
        {
            HttpHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region IRemoraResponse Members

        public IDictionary<string, string> HttpHeaders { get; private set; }

        public int StatusCode { get; set; }

        public byte[] Data { get; set; }

        #endregion
    }
}
