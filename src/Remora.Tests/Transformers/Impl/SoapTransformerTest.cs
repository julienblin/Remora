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
    }
}
