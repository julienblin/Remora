using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Remora.Core;
using Remora.Exceptions;

namespace Remora.Components
{
    public static class SoapHelper
    {
        public static XDocument GetSoapDocument(IRemoraMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Contract.EndContractBlock();

            try
            {
                return XDocument.Parse(message.ContentEncoding.GetString(message.Data));
            }
            catch (Exception ex)
            {
                throw new SoapHelperException(string.Format("There has been a error while reading soap document from {0}.", message), ex);
            }
        }

        public static string GetSoapAction(XDocument soapDocument)
        {
            
        }
    }
}
