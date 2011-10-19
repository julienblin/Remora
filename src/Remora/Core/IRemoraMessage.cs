using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Core
{
    public interface IRemoraMessage
    {
        Uri Uri { get; set; }

        Encoding ContentEncoding { get; set; }

        byte[] Data { get; set; }
    }
}
