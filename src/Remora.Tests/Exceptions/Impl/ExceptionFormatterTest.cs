using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Exceptions.Impl;

namespace Remora.Tests.Exceptions.Impl
{
    [TestFixture]
    public class ExceptionFormatterTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            var formatter = new ExceptionFormatter();

            Assert.That(() => formatter.WriteException(null, new HttpResponse(new StringWriter())),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("operation")
            );

            Assert.That(() => formatter.WriteException(new RemoraOperation(), null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("response")
            );
        }

        [Test]
        public void It_should_write_faults_when_operation_is_kind_of_soap()
        {
            var operation = new RemoraOperation
            {
                Kind = RemoraOperationKind.Soap,
                Exception = new InvalidConfigurationException("themessage")
            };

            using(var writer = new StringWriter())
            {
                var response = new HttpResponse(writer);
                var formatter = new ExceptionFormatter();

                formatter.WriteException(operation, response);

                Assert.That(response.ContentType, Is.EqualTo("text/xml"));
                Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(response.ContentEncoding, Is.EqualTo(Encoding.UTF8));

                var body = writer.ToString();
                Assert.That(body, Contains.Substring("InvalidConfiguration"));
                Assert.That(body, Contains.Substring("themessage"));

                using (var reader = new StringReader(body))
                {
                    Assert.That(() => XDocument.Load(reader), Throws.Nothing);
                }
            }
        }

        [Test]
        public void It_should_write_html_errors_when_unknown()
        {
            var operation = new RemoraOperation
            {
                Kind = RemoraOperationKind.Unknown,
                Exception = new InvalidConfigurationException("themessage")
            };

            using (var writer = new StringWriter())
            {
                var response = new HttpResponse(writer);
                var formatter = new ExceptionFormatter();

                formatter.WriteException(operation, response);

                Assert.That(response.ContentType, Is.EqualTo("text/html"));
                Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
                Assert.That(response.ContentEncoding, Is.EqualTo(Encoding.UTF8));

                var body = writer.ToString();
                Assert.That(body, Contains.Substring("InvalidConfiguration"));
                Assert.That(body, Contains.Substring("themessage"));
            }
        }
    }
}
