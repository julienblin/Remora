using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Pipeline;
using Remora.Pipeline.Impl;

namespace Remora.Tests
{
    [TestFixture]
    public class BootstraperTest : BaseTest
    {
        [Test]
        public void It_should_register_default_components()
        {
            Bootstraper.Init(Component.For<BootstraperTest>()); // Avoids loading from app.config file.

            Assert.That(Bootstraper.Container.Resolve<IRemoraOperation>(), Is.TypeOf<RemoraOperation>());
            Assert.That(Bootstraper.Container.Resolve<IPipelineEngine>(), Is.TypeOf<PipelineEngine>());
        }
    }
}
