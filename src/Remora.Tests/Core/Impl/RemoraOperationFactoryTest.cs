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
using Remora.Configuration.Impl;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class RemoraOperationFactoryTest : BaseTest
    {
        [Test]
        public void It_should_validate_inputs()
        {
            Assert.That(() => new RemoraOperationFactory(null, new RemoraConfig()),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("kernel"));

            var container = new WindsorContainer();

            Assert.That(() => new RemoraOperationFactory(container.Kernel, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("config"));

            var factory = new RemoraOperationFactory(container.Kernel, new RemoraConfig());

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

            var uri = new Uri("http://tempuri.org/uri/?foo=bar");
            var headers = new NameValueCollection {{"Content-Type", "text/xml"}};
            var sampleStream = LoadSample("SimpleHelloWorldRequest.xml");

            var factory = new RemoraOperationFactory(container.Kernel, new RemoraConfig());

            var result = factory.InternalGet(uri, headers, sampleStream);

            Assert.That(result.IncomingRequest.Uri, Is.EqualTo(uri));
            Assert.That(result.Request.Uri, Is.EqualTo(uri));
            Assert.That(result.Request.HttpHeaders.Count(), Is.EqualTo(1));
            Assert.That(result.Request.HttpHeaders.First().Key, Is.EqualTo("Content-Type"));
            Assert.That(result.Request.HttpHeaders.First().Value, Is.EqualTo("text/xml"));

            Assert.That(result.Request.Data, Is.EqualTo(LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)));
        }

        [Test]
        public void It_should_throw_an_InvalidConfigurationException_when_IRemoraOperation_is_not_registered()
        {
            var container = new WindsorContainer();
            var factory = new RemoraOperationFactory(container.Kernel, new RemoraConfig());

            Assert.That(() => factory.InternalGet(new Uri("http://tempuri.org"), new NameValueCollection(), null),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("IRemoraOperation"));
        }
    }
}
