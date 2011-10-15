using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Remora.Configuration;
using Remora.Core.Impl;
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

            var operation1 = new RemoraOperation {IncomingRequest = {Uri = "/foo/something"}};
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

            var pipelineConfig1 = new PipelineConfiguration { Id = "pipe1", UriFilterRegex = "/bar/(.*)", UriRewriteRegex = "http://tempuri.org/$1", Components = new List<string>() };
            var pipelineConfig2 = new PipelineConfiguration { Id = "pipe2", UriFilterRegex = "/(.*)", UriRewriteRegex = "http://tempuri.org/all/$1", Components = new List<string> { "cmpOne", "cmpTwo" } };
            var pipelineConfig3 = new PipelineConfiguration { Id = "pipe3", UriFilterRegex = "/foo/(.*)", UriRewriteRegex = "http://tempuri.org/all/$1", Components = new List<string>() };

            var factory = new PipelineFactory(container.Kernel, new[] { pipelineConfig1, pipelineConfig2, pipelineConfig3 }) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingRequest = { Uri = "/foo/something" } };
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
            var pipelineConfig1 = new PipelineConfiguration { Id = "pipe1", UriFilterRegex = "/bar/(.*)", UriRewriteRegex = "http://tempuri.org/$1", Components = new List<string>() };
            var pipelineConfig2 = new PipelineConfiguration { Id = "pipe2", UriFilterRegex = "/(.*)", UriRewriteRegex = "", Components = new List<string>() };

            var factory = new PipelineFactory(container.Kernel, new[] { pipelineConfig1, pipelineConfig2 }) { Logger = GetConsoleLogger() };

            var operation1 = new RemoraOperation { IncomingRequest = { Uri = "/bar/foobar?welcome" } };
            var result1 = factory.Get(operation1);

            Assert.That(operation1.OutgoingRequest.Uri, Is.EqualTo("http://tempuri.org/foobar?welcome"));

            var operation2 = new RemoraOperation { IncomingRequest = { Uri = "/foo" } };
            var result2 = factory.Get(operation2);

            Assert.That(operation2.OutgoingRequest.Uri, Is.Null);
        }

        [Test]
        public void It_should_return_null_when_not_found()
        {
            var container = new WindsorContainer();
            var factory = new PipelineFactory(container.Kernel, new PipelineConfiguration[0]) { Logger = GetConsoleLogger() };
            
            var operation = new RemoraOperation { IncomingRequest = { Uri = "/bar/foobar?welcome" } };
            Assert.That(factory.Get(operation), Is.Null);
        }
    }
}
