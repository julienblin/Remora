using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Remora.Configuration;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora.Tests.Pipeline.Impl
{
    [TestFixture]
    public class PipelineFactoryTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => new PipelineFactory(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("kernel")
            );

            var container = new WindsorContainer();

            Assert.That(() => new PipelineFactory(container.Kernel, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("pipelineConfigs")
            );

            var factory = new PipelineFactory(container.Kernel) { Logger = GetConsoleLogger() };

            Assert.That(() => factory.Get(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("operation")
            );
        }

        [Test]
        public void It_should_create_an_appropriate_pipeline()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentOne>().Named("cmpOne"));
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentTwo>().Named("cmpTwo"));

            var pipelineConfig1 = new PipelineConfiguration { Id = "pipe1", UriFilterRegex = "/foo/(.*)", UriRewriteRegex = "http://tempuri.org/{1}", Components = new List<string>() };
            var pipelineConfig2 = new PipelineConfiguration { Id = "pipe2", UriFilterRegex = "/(.*)", UriRewriteRegex = "http://tempuri.org/all/{1}", Components = new List<string> { "cmpOne", "cmpTwo" } };

            var factory = new PipelineFactory(container.Kernel, new [] { pipelineConfig1, pipelineConfig2 }) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation {IncomingUri = new Uri("http://tempuri.org/foo/something")};
            var result1 = factory.Get(operation1);

            Assert.That(result1.Id, Is.EqualTo(pipelineConfig1.Id));
            Assert.That(result1.Components.Count(), Is.EqualTo(0));
        }

        [Test]
        public void It_should_select_the_first_available_when_ambiguous()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentOne>().Named("cmpOne"));
            container.Register(Component.For<IPipelineComponent>().ImplementedBy<TestComponentTwo>().Named("cmpTwo"));

            var pipelineConfig1 = new PipelineConfiguration { Id = "pipe1", UriFilterRegex = "/bar/(.*)", UriRewriteRegex = "", Components = new List<string>() };
            var pipelineConfig2 = new PipelineConfiguration { Id = "pipe2", UriFilterRegex = "/(.*)", UriRewriteRegex = "", Components = new List<string> { "cmpOne", "cmpTwo" } };
            var pipelineConfig3 = new PipelineConfiguration { Id = "pipe3", UriFilterRegex = "/foo/(.*)", UriRewriteRegex = "", Components = new List<string>() };

            var factory = new PipelineFactory(container.Kernel, new[] { pipelineConfig1, pipelineConfig2, pipelineConfig3 }) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/foo/something") };
            var result1 = factory.Get(operation1);

            Assert.That(result1.Id, Is.EqualTo(pipelineConfig2.Id));
            Assert.That(result1.Components.Count(), Is.EqualTo(2));
            Assert.That(result1.Components.First(), Is.TypeOf<TestComponentOne>());
            Assert.That(result1.Components.Skip(1).First(), Is.TypeOf<TestComponentTwo>());
        }

        [Test]
        public void It_should_rewrite_url_if_present()
        {
            var container = new WindsorContainer();
            var pipelineConfig1 = new PipelineConfiguration { Id = "pipe1", UriFilterRegex = "http(s)?://.*/bar/(.*)", UriRewriteRegex = "http://tempuri2.org/$2", Components = new List<string>() };
            var pipelineConfig2 = new PipelineConfiguration { Id = "pipe2", UriFilterRegex = "/(.*)", UriRewriteRegex = "", Components = new List<string>() };

            var factory = new PipelineFactory(container.Kernel, new[] { pipelineConfig1, pipelineConfig2 }) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/bar/foobar?welcome") };
            var result1 = factory.Get(operation1);

            Assert.That(operation1.Request.Uri, Is.EqualTo(new Uri("http://tempuri2.org/foobar?welcome")));

            var operation2 = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/foo") };
            var result2 = factory.Get(operation2);

            Assert.That(operation2.Request.Uri, Is.Null);
        }

        [Test]
        public void It_should_return_null_when_not_found()
        {
            var container = new WindsorContainer();
            var factory = new PipelineFactory(container.Kernel, new PipelineConfiguration[0]) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/bar/foobar?welcome") };
            Assert.That(factory.Get(operation), Is.Null);
        }

        [Test]
        public void It_should_throw_InvalidConfigurationException_when_filter_regex_is_invalid()
        {
            var container = new WindsorContainer();
            var pipelineConfig = new PipelineConfiguration { Id = "pipe", UriFilterRegex = "(((", UriRewriteRegex = "", Components = new List<string>() };
            var factory = new PipelineFactory(container.Kernel, new PipelineConfiguration[] { pipelineConfig }) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/") };
            Assert.That(() => factory.Get(operation),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("(((")
            );
        }

        [Test]
        public void It_should_throw_InvalidConfigurationException_when_component_not_registered()
        {
            var container = new WindsorContainer();
            var pipelineConfig = new PipelineConfiguration { Id = "pipe", UriFilterRegex = ".*", UriRewriteRegex = "", Components = new List<string> { "comp1" } };
            var factory = new PipelineFactory(container.Kernel, new PipelineConfiguration[] { pipelineConfig }) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/") };
            Assert.That(() => factory.Get(operation),
                Throws.Exception.TypeOf<InvalidConfigurationException>()
                .With.Message.Contains("comp1")
            );
        }

        [Test]
        public void It_should_throw_UrlRewriteException_when_urlrewriting_fails()
        {
            var container = new WindsorContainer();
            var pipelineConfig = new PipelineConfiguration { Id = "pipe", UriFilterRegex = ".*", UriRewriteRegex = "foo", Components = new List<string>() };
            var factory = new PipelineFactory(container.Kernel, new PipelineConfiguration[] { pipelineConfig }) { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org/") };
            Assert.That(() => factory.Get(operation),
                Throws.Exception.TypeOf<UrlRewriteException>()
                .With.Message.Contains(".*")
                .And.Message.Contains("foo")
            );
        }
    }
}
