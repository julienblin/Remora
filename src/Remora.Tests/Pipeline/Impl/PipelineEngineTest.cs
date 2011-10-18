using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration;
using Remora.Configuration.Impl;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora.Tests.Pipeline.Impl
{
    [TestFixture]
    public class PipelineEngineTest : BaseTest
    {
        public class PcOne : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback();
            }
        }

        public class PcTwo : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback();
            }
        }

        public class LastPc : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback(false);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback();
            }
        }

        public class PcWithErrorOnBegin : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public Exception Exception { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                throw Exception;
            }

            public override void EndAsyncProcess(IRemoraOperation operation, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback();
            }
        }

        public class PcWithErrorOnEnd : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public Exception Exception { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                throw Exception;
            }
        }

        [Test]
        public void It_should_handle_sync_errors_on_Begin()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IRemoraConfig>().Instance(new RemoraConfig()),
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.SenderComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();
            var exception = new Exception();

            var pcErrorBegin = new PcWithErrorOnBegin { ControlOperation = operation, Exception = exception };
            var pcOne = new PcOne { ControlOperation = operation };

            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcErrorBegin, pcOne });

            engine.RunAsync(operation, pipeline, (op) =>
                                                     {
                                                         Assert.That(operation.OnError);
                                                         Assert.That(operation.Exception, Is.SameAs(exception));
                                                         Assert.That(pcErrorBegin.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcErrorBegin.EndAsyncProcessCalledCount, Is.EqualTo(0));
                                                         Assert.That(pcOne.BeginAsyncProcessCalledCount, Is.EqualTo(0));
                                                         Assert.That(pcOne.EndAsyncProcessCalledCount, Is.EqualTo(0));
                                                     });
        }

        [Test]
        public void It_should_handle_sync_errors_on_End()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IRemoraConfig>().Instance(new RemoraConfig()), 
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.SenderComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();
            var exception = new Exception();

            var pcOne = new PcOne { ControlOperation = operation };
            var pcErrorLast = new PcWithErrorOnEnd { ControlOperation = operation, Exception = exception };
            var pcLast = new LastPc { ControlOperation = operation };

            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcOne, pcErrorLast, pcLast });

            engine.RunAsync(operation, pipeline, (op) =>
                                                     {
                                                         Assert.That(operation.OnError);
                                                         Assert.That(operation.Exception, Is.SameAs(exception));
                                                         Assert.That(pcOne.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcOne.EndAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcErrorLast.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcErrorLast.EndAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcLast.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcLast.EndAsyncProcessCalledCount, Is.EqualTo(0));
                                                     });
        }

        [Test]
        public void It_should_invoke_pipeline_components()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IRemoraConfig>().Instance(new RemoraConfig()),
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.SenderComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();

            var pcOne = new PcOne { ControlOperation = operation };
            var pcTwo = new PcTwo { ControlOperation = operation };
            var pcLast = new LastPc { ControlOperation = operation };

            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcOne, pcTwo, pcLast });

            engine.RunAsync(operation, pipeline, (op) =>
                                                     {
                                                         Assert.That(pcOne.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcOne.EndAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcTwo.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcTwo.EndAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcLast.BeginAsyncProcessCalledCount, Is.EqualTo(1));
                                                         Assert.That(pcLast.EndAsyncProcessCalledCount, Is.EqualTo(0));
                                                     });
        }

        [Test]
        public void It_should_validate_arguments()
        {
            var engine = new PipelineEngine { Logger = GetConsoleLogger() };
            var operation = new RemoraOperation();

            Assert.That(() => engine.RunAsync(null, new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[0]), (op) => { }),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation")
                );

            Assert.That(() => engine.RunAsync(operation, null, (op) => { }),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("pipeline")
                );

            Assert.That(() => engine.RunAsync(operation, new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[0]), null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("callback")
                );
        }
    }
}
