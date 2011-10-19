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
            if(message == null) throw new ArgumentNullException("message");
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
    }
}
