﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Remora.Core
{
    /// <summary>
    /// Abstract request
    /// </summary>
    public interface IRemoraRequest
    {
        Uri Uri { get; set; }

        IDictionary<string, string> HttpHeaders { get; }

        string Method { get; set; }

        byte[] Data { get; set; }
    }
}