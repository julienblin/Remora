using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Castle.Core.Logging;
using Remora.Core;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Transformers.Impl
{
    public class SoapTransformer : ISoapTransformer
    {
        public const string SoapEnvelopeNamespace = @"http://schemas.xmlsoap.org/soap/envelope/";
        public const string SoapEnvelopeNamespaceLinq = "{" + SoapEnvelopeNamespace + "}";
        public const string SoapHeadersLinq = SoapEnvelopeNamespaceLinq + "Header";
        public const string SoapBodyLinq = SoapEnvelopeNamespaceLinq + "Body";

        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public XDocument LoadSoapDocument(IRemoraMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Contract.EndContractBlock();

            try
            {
                if(Logger.IsDebugEnabled)
                    Logger.DebugFormat("Loading soap document from {0} with content encoding {1}...", message.Uri, message.ContentEncoding.EncodingName);

                return XDocument.Parse(message.GetDataAsString());
            }
            catch (Exception ex)
            {
                throw new SoapTransformerException(string.Format("Unable to create a XDocument from message {0}.", message), ex);
            }
        }

        public void SaveSoapDocument(IRemoraMessage message, XDocument soapDocument)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (soapDocument == null) throw new ArgumentNullException("soapDocument");
            Contract.EndContractBlock();

            message.SetData(soapDocument.ToString());
        }

        public XElement GetHeaders(XDocument soapDocument)
        {
            if (soapDocument == null) throw new ArgumentNullException("soapDocument");
            Contract.EndContractBlock();

            return soapDocument.Descendants(SoapHeadersLinq).FirstOrDefault();
        }

        public XElement GetBody(XDocument soapDocument)
        {
            if (soapDocument == null) throw new ArgumentNullException("soapDocument");
            Contract.EndContractBlock();

            return soapDocument.Descendants(SoapBodyLinq).FirstOrDefault();
        }

        public string GetSoapActionName(XDocument soapDocument)
        {
            if (soapDocument == null) throw new ArgumentNullException("soapDocument");
            Contract.EndContractBlock();

            var body = GetBody(soapDocument);
            if (body == null)
                return null;

            var firstChild = body.Descendants().FirstOrDefault();
            if (firstChild == null)
                return null;

            return firstChild.Name.LocalName;
        }
    }
}
