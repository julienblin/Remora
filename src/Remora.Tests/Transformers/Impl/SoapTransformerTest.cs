using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Extensions;
using Remora.Transformers.Impl;

namespace Remora.Tests.Transformers.Impl
{
    [TestFixture]
    public class SoapTransformerTest : BaseTest
    {
        private SoapTransformer _transformer;

        [SetUp]
        public void SetUp()
        {
            _transformer = new SoapTransformer { Logger = GetConsoleLogger() };
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => _transformer.LoadSoapDocument(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("message"));

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
    }
}
