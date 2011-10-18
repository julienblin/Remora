using NUnit.Framework;
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
        }
    }
}
