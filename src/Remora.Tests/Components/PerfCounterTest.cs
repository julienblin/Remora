using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration.Impl;
using Remora.Core.Impl;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class PerfCounterTest : BaseTest
    {
        [Test]
        public void It_should_be_able_to_create_and_retrieve_performance_counters()
        {
            const string instanceName = "InstanceName";

            try
            {
                PerformanceCounterCategory.Delete(PerfCounter.DefaultPerfCounterCategory);
            }
            catch { }

            var factory = new PerfCounterFactory { Logger = GetConsoleLogger() };
            Assert.That(factory.GetPerfCounter(PerfCounterDefinition.NumMessagesHandled, instanceName, PerfCounter.DefaultPerfCounterCategory),
                Is.Not.Null.And.Property("InstanceName").EqualTo(instanceName));
            Assert.That(factory.GetPerfCounter(PerfCounterDefinition.NumMessagesHandledPerSec, instanceName, PerfCounter.DefaultPerfCounterCategory),
                Is.Not.Null.And.Property("InstanceName").EqualTo(instanceName));
            Assert.That(factory.GetPerfCounter(PerfCounterDefinition.AverageDurationForMessageHandling, instanceName, PerfCounter.DefaultPerfCounterCategory),
                Is.Not.Null.And.Property("InstanceName").EqualTo(instanceName));
            Assert.That(factory.GetPerfCounter(PerfCounterDefinition.AverageDurationForMessageHandlingBase, instanceName, PerfCounter.DefaultPerfCounterCategory),
                Is.Not.Null.And.Property("InstanceName").EqualTo(instanceName));
        }

        [Test]
        public void It_should_let_messages_go_through()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org")
            };

            var componentDefinition = new ComponentDefinition();

            var perfCounter = new PerfCounter { Logger = GetConsoleLogger() };

            Assert.That(() => perfCounter.BeginAsyncProcess(operation, componentDefinition, (b) =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
            }), Throws.Nothing);

            Assert.That(() => perfCounter.EndAsyncProcess(operation, componentDefinition, () =>
            {
                Assert.That(!operation.OnError);
            }), Throws.Nothing);
        }
    }
}
