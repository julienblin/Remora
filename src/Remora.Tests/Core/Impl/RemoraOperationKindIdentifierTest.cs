using System;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class RemoraOperationKindIdentifierTest : BaseTest
    {
        [Test]
        public void It_should_identify_soap_requests_by_SOAPAction_header()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };
            var operation = new RemoraOperation();
            operation.Request.HttpHeaders.Add("SOAPAction", "http://tempuri.org/");

            Assert.That(identifier.Identify(operation), Is.EqualTo(RemoraOperationKind.Soap));
        }

        [Test]
        public void It_should_return_unknown()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };
            var operation = new RemoraOperation();

            Assert.That(identifier.Identify(operation), Is.EqualTo(RemoraOperationKind.Unknown));
        }

        [Test]
        public void It_should_validate_arguments()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };

            Assert.That(() => identifier.Identify(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation"));
        }
    }
}
