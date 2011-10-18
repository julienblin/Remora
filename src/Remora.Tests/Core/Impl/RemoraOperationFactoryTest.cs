#region License
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
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
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
        public void It_should_return_a_IRemoraOperation()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IRemoraOperation>().ImplementedBy<RemoraOperation>());

            var uri = new Uri("http://tempuri.org/uri/?foo=bar");
            var headers = new NameValueCollection {{"Content-Type", "text/xml"}};
            var sampleStream = LoadSample("SimpleHelloWorldRequest.xml");

            var factory = new RemoraOperationFactory(container.Kernel, new RemoraConfig()) { Logger = GetConsoleLogger() };

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
            var factory = new RemoraOperationFactory(container.Kernel, new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => factory.InternalGet(new Uri("http://tempuri.org"), new NameValueCollection(), null),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("IRemoraOperation"));
        }

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
    }
}
