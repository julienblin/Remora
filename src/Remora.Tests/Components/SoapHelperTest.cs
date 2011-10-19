using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SoapHelperTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => SoapHelper.GetSoapDocument(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("message"));
        }

        [Test]
        public void It_should_load_soap_documents_from_messages()
        {
            var request = new RemoraRequest
            {
                ContentEncoding = Encoding.UTF8,
                Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
            };

            var result = SoapHelper.GetSoapDocument(request);
            Assert.That(result.Descendants("{http://schemas.xmlsoap.org/soap/envelope/}Header").Count(), Is.EqualTo(1));
        }

        [Test]
        public void It_should_throw_a_SoapHelperException_when_soap_documents_are_wrong()
        {
            var request = new RemoraRequest
            {
                ContentEncoding = Encoding.BigEndianUnicode,
                Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0)
            };

            Assert.That(() => SoapHelper.GetSoapDocument(request),
                Throws.Exception.TypeOf<SoapHelperException>()
                .With.Message.Contains("soap"));
        }
    }
}
