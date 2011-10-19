using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Remora.Core;

namespace Remora.Transformers
{
    public interface ISoapTransformer
    {
        XDocument LoadSoapDocument(IRemoraMessage message);
    }
}
