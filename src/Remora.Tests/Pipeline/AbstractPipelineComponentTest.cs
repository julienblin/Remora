using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Pipeline;

namespace Remora.Tests.Pipeline
{
    [TestFixture]
    public class AbstractPipelineComponentTest : BaseTest
    {
        [Test]
        public void It_should_call_callback_with_true_on_BeginAsyncProcess()
        {
            var apc = new ApcTest();
            var wasCalled = false;
            apc.BeginAsyncProcess(new RemoraOperation(), (b) =>
            {
                Assert.That(b);
                wasCalled = true;
            });
            Assert.That(wasCalled);
        }

        [Test]
        public void It_should_call_callback_on_EndAsyncProcess()
        {
            var apc = new ApcTest();
            var wasCalled = false;
            apc.EndAsyncProcess(new RemoraOperation(), () =>
            {
                wasCalled = true;
            });
            Assert.That(wasCalled);
        }

        public class ApcTest : AbstractPipelineComponent {}
    }
}
