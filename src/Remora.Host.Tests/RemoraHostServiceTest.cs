using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Host.Configuration.Impl;
using Remora.Host.Exceptions;

namespace Remora.Host.Tests
{
    [TestFixture]
    public class RemoraHostServiceTest : BaseTest
    {
        [Test]
        public void It_should_validate_that_prefixes_have_been_defined()
        {
            var config = new RemoraHostConfig();
            var service = new RemoraHostService(config);

            Assert.That(() => service.Start(),
                Throws.Exception.TypeOf<RemoraHostServiceException>()
                .With.Message.Contains("prefix"));
        }
    }
}
