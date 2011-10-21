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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Extensions;
using Remora.Transformers.Impl;

namespace Remora.Tests.Transformers.Impl
{
    [TestFixture]
    public class SoapTransformerTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _transformer = new SoapTransformer {Logger = GetConsoleLogger()};
        }

        #endregion

        private SoapTransformer _transformer;

        [Test]
        public void It_should_extract_soap_action_name()
        {
            var message = new RemoraRequest
                              {
                                  ContentEncoding = Encoding.UTF8,
                                  Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
                              };

            var document = _transformer.LoadSoapDocument(message);
            Assert.That(_transformer.GetSoapActionName(document), Is.EqualTo("HelloWorldRequest"));
        }

        [Test]
        public void It_should_get_body()
        {
            var message = new RemoraRequest
                              {
                                  ContentEncoding = Encoding.UTF8,
                                  Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
                              };

            var doc = _transformer.LoadSoapDocument(message);
            var result = _transformer.GetBody(doc);
            Assert.That(result.Name.LocalName, Is.EqualTo("Body"));
        }

        [Test]
        public void It_should_get_headers()
        {
            var message = new RemoraRequest
                              {
                                  ContentEncoding = Encoding.UTF8,
                                  Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
                              };

            var doc = _transformer.LoadSoapDocument(message);
            var result = _transformer.GetHeaders(doc);
            Assert.That(result.Name.LocalName, Is.EqualTo("Header"));
        }

        [Test]
        public void It_should_load_a_soap_document()
        {
            var message = new RemoraRequest
                              {
                                  ContentEncoding = Encoding.UTF8,
                                  Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
                              };

            var result = _transformer.LoadSoapDocument(message);
            Assert.That(result.Descendants(SoapTransformer.SoapEnvelopeNamespaceLinq + "Body").Count(), Is.EqualTo(1));
        }

        [Test]
        public void It_should_save_a_soap_document()
        {
            var refDoc = XDocument.Load(LoadSample("SimpleHelloWorldRequest.xml"));

            var message = new RemoraRequest
                              {
                                  ContentEncoding = Encoding.UTF8,
                              };

            _transformer.SaveSoapDocument(message, refDoc);
            Assert.That(message.GetDataAsString(), Is.EqualTo(refDoc.ToString()));
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => _transformer.LoadSoapDocument(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("message"));

            Assert.That(() => _transformer.SaveSoapDocument(null, null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("message"));

            Assert.That(() => _transformer.SaveSoapDocument(new RemoraRequest(), null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("soapDocument"));

            Assert.That(() => _transformer.GetSoapActionName(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("soapDocument"));

            Assert.That(() => _transformer.GetHeaders(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("soapDocument"));

            Assert.That(() => _transformer.GetBody(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("soapDocument"));
        }
    }
}