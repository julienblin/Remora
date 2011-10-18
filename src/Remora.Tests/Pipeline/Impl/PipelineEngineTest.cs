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
using System.Collections.Generic;
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

            public IComponentDefinition ControlComponentDefinition { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback();
            }
        }

        public class PcTwo : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public IComponentDefinition ControlComponentDefinition { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback();
            }
        }

        public class LastPc : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public IComponentDefinition ControlComponentDefinition { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback(false);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback();
            }
        }

        public class PcWithErrorOnBegin : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public IComponentDefinition ControlComponentDefinition { get; set; }

            public Exception Exception { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                throw Exception;
            }

            public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback();
            }
        }

        public class PcWithErrorOnEnd : AbstractPipelineComponent
        {
            public IRemoraOperation ControlOperation { get; set; }

            public IComponentDefinition ControlComponentDefinition { get; set; }

            public Exception Exception { get; set; }

            public int BeginAsyncProcessCalledCount { get; set; }

            public int EndAsyncProcessCalledCount { get; set; }

            public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
            {
                ++BeginAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                callback(true);
            }

            public override void EndAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action callback)
            {
                ++EndAsyncProcessCalledCount;
                Assert.That(operation, Is.SameAs(ControlOperation));
                Assert.That(componentDefinition, Is.SameAs(ControlComponentDefinition));
                throw Exception;
            }
        }

        [Test]
        public void It_should_handle_sync_errors_on_Begin()
        {
            var container = new WindsorContainer();
            container.Register(
                Component.For<IRemoraConfig>().Instance(new RemoraConfig()),
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.ComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();
            var exception = new Exception();

            var pcErrorBegin = new PcWithErrorOnBegin { ControlOperation = operation, Exception = exception };
            var pcOne = new PcOne { ControlOperation = operation };

            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcErrorBegin, pcOne }, null);

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
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.ComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();
            var exception = new Exception();

            var pcOne = new PcOne { ControlOperation = operation };
            var pcErrorLast = new PcWithErrorOnEnd { ControlOperation = operation, Exception = exception };
            var pcLast = new LastPc { ControlOperation = operation };

            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcOne, pcErrorLast, pcLast }, null);

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
                Component.For<IPipelineComponent>().ImplementedBy<Sender>().Named(Sender.ComponentId)
                );
            var engine = new PipelineEngine { Logger = GetConsoleLogger(), Kernel = container.Kernel };
            var operation = new RemoraOperation();

            var pcOneDefinition = new ComponentDefinition();
            var pcOne = new PcOne { ControlOperation = operation, ControlComponentDefinition = pcOneDefinition};
            var pcTwoDefinition = new ComponentDefinition();
            var pcTwo = new PcTwo { ControlOperation = operation, ControlComponentDefinition = pcTwoDefinition };
            var pcLastDefinition = new ComponentDefinition();
            var pcLast = new LastPc { ControlOperation = operation, ControlComponentDefinition = pcLastDefinition };

            var pipelineDef = new PipelineDefinition { ComponentDefinitions = new List<IComponentDefinition> { pcOneDefinition, pcTwoDefinition, pcLastDefinition } };
            var pipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[] { pcOne, pcTwo, pcLast }, pipelineDef);


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

            Assert.That(() => engine.RunAsync(null, new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[0], null), (op) => { }),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation")
                );

            Assert.That(() => engine.RunAsync(operation, null, (op) => { }),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("pipeline")
                );

            Assert.That(() => engine.RunAsync(operation, new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[0], null), null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("callback")
                );
        }
    }
}
