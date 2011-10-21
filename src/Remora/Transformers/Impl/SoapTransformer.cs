#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Diagnostics.Contracts;
using System.Linq;
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
        ///   Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #region ISoapTransformer Members

        public XDocument LoadSoapDocument(IRemoraMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Contract.EndContractBlock();

            try
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Loading soap document from {0} with content encoding {1}...", message.Uri,
                                       message.ContentEncoding.EncodingName);

                return XDocument.Parse(message.GetDataAsString());
            }
            catch (Exception ex)
            {
                throw new SoapTransformerException(
                    string.Format("Unable to create a XDocument from message {0}.", message), ex);
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

        #endregion
    }
}