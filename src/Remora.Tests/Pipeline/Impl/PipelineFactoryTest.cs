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
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Remora.Configuration;
using Remora.Configuration.Impl;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora.Tests.Pipeline.Impl
{
    [TestFixture]
    public class PipelineFactoryTest : BaseTest
    {
        private IPipelineDefinition PDef(string id, string filter, string rewrite, params string[] componentRef)
        {
            return new PipelineDefinition
                       {
                           Id = id,
                           UriFilterRegex = filter,
                           UriRewriteRegex = rewrite,
                           ComponentDefinitions = componentRef.Select(cmpRef => new ComponentDefinition { RefId = cmpRef })
                       };
        }

        [Test]
        public void It_should_create_an_appropriate_pipeline()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentOne>().Named("cmpOne"));
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentTwo>().Named("cmpTwo"));

            var pipelineDef1 = PDef("pipe1", "/foo/(.*)", "");
            var pipelineDef2 = PDef("pipe2", "/(.*)", "", "cmpOne", "cmpTwo");
            var config = new RemoraConfig { PipelineDefinitions = new [] { pipelineDef1, pipelineDef2 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/foo/something")} };
            var result1 = factory.Get(operation1);

            Assert.That(result1.Id, Is.EqualTo(pipelineDef1.Id));
            Assert.That(result1.Components.Count(), Is.EqualTo(0));
            Assert.That(operation1.ExecutingPipeline, Is.SameAs(result1));
        }

        [Test]
        public void It_should_return_null_when_not_found()
        {
            var container = new WindsorContainer();
            var factory = new PipelineFactory(container.Kernel, new RemoraConfig()) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/bar/foobar?welcome")} };
            Assert.That(factory.Get(operation), Is.Null);
        }

        [Test]
        public void It_should_rewrite_url_if_present()
        {
            var container = new WindsorContainer();

            var pipelineDef1 = PDef("pipe1", "http(s)?://.*/bar/(.*)", "http://tempuri2.org/$2");
            var pipelineDef2 = PDef("pipe2", "/(.*)", "");
            var config = new RemoraConfig { PipelineDefinitions = new[] { pipelineDef1, pipelineDef2 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/bar/foobar?welcome")} };
            var result1 = factory.Get(operation1);

            Assert.That(operation1.Request.Uri, Is.EqualTo(new Uri("http://tempuri2.org/foobar?welcome")));

            var operation2 = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/foo")} };
            var result2 = factory.Get(operation2);

            Assert.That(operation2.Request.Uri, Is.Null);
        }

        [Test]
        public void It_should_select_the_first_available_when_ambiguous()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentOne>().Named("cmpOne"));
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentTwo>().Named("cmpTwo"));

            var pipelineDef1 = PDef("pipe1", "/bar/(.*)", "");
            var pipelineDef2 = PDef("pipe2", "/(.*)", "", "cmpOne", "cmpTwo");
            var pipelineDef3 = PDef("pipe3", "/foo/(.*)", "", "cmpOne", "cmpTwo");
            var config = new RemoraConfig { PipelineDefinitions = new[] { pipelineDef1, pipelineDef2, pipelineDef3 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/foo/something")} };
            var result1 = factory.Get(operation1);

            Assert.That(result1.Id, Is.EqualTo(pipelineDef2.Id));
            Assert.That(result1.Components.Count(), Is.EqualTo(2));
            Assert.That(result1.Components.First(), Is.TypeOf<TestComponentOne>());
            Assert.That(result1.Components.Skip(1).First(), Is.TypeOf<TestComponentTwo>());
        }

        [Test]
        public void It_should_throw_InvalidConfigurationException_when_component_not_registered()
        {
            var container = new WindsorContainer();

            var pipelineDef1 = PDef("pipe", ".*", "", "comp1");
            var config = new RemoraConfig { PipelineDefinitions = new[] { pipelineDef1 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/")} };
            Assert.That(() => factory.Get(operation),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("comp1")
            );
        }

        [Test]
        public void It_should_throw_InvalidConfigurationException_when_filter_regex_is_invalid()
        {
            var container = new WindsorContainer();

            var pipelineDef1 = PDef("pipe", "(((", "");
            var config = new RemoraConfig { PipelineDefinitions = new[] { pipelineDef1 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/")} };
            Assert.That(() => factory.Get(operation),
                        Throws.Exception.TypeOf<InvalidConfigurationException>()
                            .With.Message.Contains("(((")
                );
        }

        [Test]
        public void It_should_throw_UrlRewriteException_when_urlrewriting_fails()
        {
            var container = new WindsorContainer();

            var pipelineDef1 = PDef("pipe", ".*", "foo");
            var config = new RemoraConfig { PipelineDefinitions = new[] { pipelineDef1 } };

            var factory = new PipelineFactory(container.Kernel, config) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org/")} };
            Assert.That(() => factory.Get(operation),
                Throws.Exception.TypeOf<UrlRewriteException>()
                .With.Message.Contains(".*")
                .And.Message.Contains("foo")
            );
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => new PipelineFactory(null, new RemoraConfig()),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("kernel")
                );

            var container = new WindsorContainer();

            Assert.That(() => new PipelineFactory(container.Kernel, null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("config")
                );

            var factory = new PipelineFactory(container.Kernel, new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => factory.Get(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation")
                );
        }
    }
}
