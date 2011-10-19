using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Remora.Core;

namespace Remora.Transformers.Impl
{
    public class SoapTransformer : ISoapTransformer
    {
        public XDocument LoadSoapDocument(IRemoraMessage message)
        {
            if(message == null) throw new ArgumentNullException("message");
            Contract.EndContractBlock();

            throw new NotImplementedException();
        }
    }
}
