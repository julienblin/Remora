using System.Collections.Generic;

namespace Remora.Core
{
    /// <summary>
    /// Abstract response
    /// </summary>
    public interface IRemoraResponse
    {
        IDictionary<string, string> HttpHeaders { get; }

        int StatusCode { get; set; }

        byte[] Data { get; set; }
    }
}