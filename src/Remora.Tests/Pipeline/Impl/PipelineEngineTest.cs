using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora.Tests.Pipeline.Impl
{
    [TestFixture]
    public class PipelineEngineTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            var engine = new PipelineEngine { Logger = GetConsoleLogger() };

            Assert.That(() => engine.Run(null, new Remora.Pipeline.Impl.Pipeline("default", ".*", "")),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("operation")
            );

            Assert.That(() => engine.Run(new RemoraOperation(), null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("pipeline")
            );
        }

        [Test]
        public void It_should_invoke_the_pipeline_components_with_the_operation()
        {
            var operation = new RemoraOperation();
            operation.IncomingRequest.Uri = @"http://www.example.org/request";
            var components = new[] { new TestPipelineComponent(), new TestPipelineComponent(), new TestPipelineComponent() };
            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", ".*", "", components);

            foreach (var component in components)
            {
                component.ComparePipeline = pipeline;
                component.CompareOperation = operation;
            }

            var engine = new PipelineEngine { Logger = GetConsoleLogger() };

            engine.Run(operation, pipeline);

            foreach (var component in components)
            {
                Assert.That(component.HasBeenInvoked);
            }
        }

        public class TestPipelineComponent : IPipelineComponent
        {
            public bool HasBeenInvoked { get; private set; }

            public IRemoraOperation CompareOperation { get; set; }
            public IPipeline ComparePipeline { get; set; }

            public string Id
            {
                get { return "TestPipelineComponent"; }
                set {}
            }

            public void Proceed(IPipelineComponentInvocation invocation)
            {
                HasBeenInvoked = true;
                Assert.That(invocation.Operation, Is.SameAs(CompareOperation));
                Assert.That(invocation.Pipeline, Is.SameAs(ComparePipeline));
                invocation.ProceedWithNextComponent();
            }
        }
    }
}
