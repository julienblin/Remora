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

using NUnit.Framework;
using Remora.Components;
using Remora.Configuration;
using Remora.Configuration.Impl;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Exceptions.Impl;
using Remora.Handler;
using Remora.Handler.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;
using Remora.Transformers;
using Remora.Transformers.Impl;

namespace Remora.Tests
{
    [TestFixture]
    public class BootstraperTest : BaseTest
    {
        [Test]
        public void It_should_init_only_once()
        {
            Bootstraper.Init();
            var container = Bootstraper.Container;

            Bootstraper.Init();
            Assert.That(Bootstraper.Container, Is.SameAs(container));
        }

        [Test]
        public void It_should_register_default_components()
        {
            Bootstraper.Init();

            Assert.That(Bootstraper.Container.Resolve<IRemoraOperation>(), Is.TypeOf<RemoraOperation>());
            Assert.That(Bootstraper.Container.Resolve<IRemoraOperation>(), Is.Not.SameAs(Bootstraper.Container.Resolve<IRemoraOperation>()));

            Assert.That(Bootstraper.Container.Resolve<IPipelineEngine>(), Is.TypeOf<PipelineEngine>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineEngine>(), Is.SameAs(Bootstraper.Container.Resolve<IPipelineEngine>()));

            Assert.That(Bootstraper.Container.Resolve<IRemoraOperationKindIdentifier>(), Is.TypeOf<RemoraOperationKindIdentifier>());
            Assert.That(Bootstraper.Container.Resolve<IRemoraOperationKindIdentifier>(), Is.SameAs(Bootstraper.Container.Resolve<IRemoraOperationKindIdentifier>()));

            Assert.That(Bootstraper.Container.Resolve<IRemoraOperationFactory>(), Is.TypeOf<RemoraOperationFactory>());
            Assert.That(Bootstraper.Container.Resolve<IRemoraOperationFactory>(), Is.SameAs(Bootstraper.Container.Resolve<IRemoraOperationFactory>()));

            Assert.That(Bootstraper.Container.Resolve<IRemoraConfig>(), Is.TypeOf<RemoraConfig>());

            Assert.That(Bootstraper.Container.Resolve<IPipelineFactory>(), Is.TypeOf<PipelineFactory>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineFactory>(), Is.SameAs(Bootstraper.Container.Resolve<IPipelineFactory>()));

            Assert.That(Bootstraper.Container.Resolve<IExceptionFormatter>(), Is.TypeOf<ExceptionFormatter>());
            Assert.That(Bootstraper.Container.Resolve<IExceptionFormatter>(), Is.SameAs(Bootstraper.Container.Resolve<IExceptionFormatter>()));

            Assert.That(Bootstraper.Container.Resolve<IResponseWriter>(), Is.TypeOf<ResponseWriter>());
            Assert.That(Bootstraper.Container.Resolve<IResponseWriter>(), Is.SameAs(Bootstraper.Container.Resolve<IResponseWriter>()));

            Assert.That(Bootstraper.Container.Resolve<ISoapTransformer>(), Is.TypeOf<SoapTransformer>());
            Assert.That(Bootstraper.Container.Resolve<ISoapTransformer>(), Is.SameAs(Bootstraper.Container.Resolve<ISoapTransformer>()));

            Assert.That(Bootstraper.Container.Resolve<IPipelineComponent>(Sender.ComponentId), Is.TypeOf<Sender>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineComponent>(SoapRecorder.ComponentId), Is.TypeOf<SoapRecorder>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineComponent>(SoapPlayer.ComponentId), Is.TypeOf<SoapPlayer>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineComponent>(SetHttpHeader.ComponentId), Is.TypeOf<SetHttpHeader>());
        }
    }
}
