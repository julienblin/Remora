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
using System.IO;
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
