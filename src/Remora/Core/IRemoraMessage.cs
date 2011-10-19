using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Core
{
    public interface IRemoraMessage
    {
        Encoding ContentEncoding { get; set; }

        byte[] Data { get; set; }
    }
}
