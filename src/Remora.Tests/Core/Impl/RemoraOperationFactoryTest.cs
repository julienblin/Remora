using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class RemoraOperationFactoryTest : BaseTest
    {
        [Test]
        public void It_should_validate_inputs()
        {
            Assert.That(() => new RemoraOperationFactory(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("kernel"));

            var container = new WindsorContainer();
            var factory = new RemoraOperationFactory(container.Kernel);

            Assert.That(() => factory.Get((HttpRequest)null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("request"));

            Assert.That(() => factory.Get((HttpListenerRequest)null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("request"));
        }

        [Test]
        public void It_should_return_a_IRemoraOperation()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IRemoraOperation>().ImplementedBy<RemoraOperation>());

            var uri = "/uri/?foo=bar";
            var headers = new NameValueCollection();
            headers.Add("Content-Type", "text/xml");
            var sampleStream = LoadSample("SimpleHelloWorldRequest.xml");

            var factory = new RemoraOperationFactory(container.Kernel);

            var result = factory.InternalGet(uri, headers, sampleStream);

            Assert.That(result.IncomingRequest.Uri, Is.EqualTo(uri));
            Assert.That(result.IncomingRequest.HttpHeaders.Count(), Is.EqualTo(1));
            Assert.That(result.IncomingRequest.HttpHeaders.First().Key, Is.EqualTo("Content-Type"));
            Assert.That(result.IncomingRequest.HttpHeaders.First().Value, Is.EqualTo("text/xml"));

            var testDoc = XDocument.Load(LoadSample("SimpleHelloWorldRequest.xml"));
            var testHeader = testDoc.Descendants("{" + RemoraOperationFactory.SoapEnvelopeSchema + "}Header").FirstOrDefault();
            var testBody = testDoc.Descendants("{" + RemoraOperationFactory.SoapEnvelopeSchema + "}Body").FirstOrDefault();
            Assert.That(result.IncomingRequest.SoapPayload.ToString(), Is.EqualTo(testDoc.ToString()));
            Assert.That(result.IncomingRequest.SoapHeaders.ToString(), Is.EqualTo(testHeader.ToString()));
            Assert.That(result.IncomingRequest.SoapBody.ToString(), Is.EqualTo(testBody.ToString()));
        }

        [Test]
        public void It_should_throw_an_InvalidConfigurationException_when_IRemoraOperation_is_not_registered()
        {
            var container = new WindsorContainer();
            var factory = new RemoraOperationFactory(container.Kernel);

            Assert.That(() => factory.InternalGet("", new NameValueCollection(), null),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("IRemoraOperation"));
        }

        [Test]
        public void It_should_throw_an_SoapParsingException_when_Soap_is_not_correct()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IRemoraOperation>().ImplementedBy<RemoraOperation>());
            var factory = new RemoraOperationFactory(container.Kernel);

            var sampleStream = LoadSample("InvalidRequest.xml");

            Assert.That(() => factory.InternalGet("", new NameValueCollection(), sampleStream),
                Throws.Exception.TypeOf<SoapParsingException>());
        }
    }
}
