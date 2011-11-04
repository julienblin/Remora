using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using Remora.Core.Impl;
using Rhino.Mocks;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class UniversalResponseTest : BaseTest
    {
        private MockRepository _mocks;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        [Test]
        public void It_should_work_with_HttpResponse()
        {
            var response = _mocks.Stub<HttpResponseBase>();
            _mocks.Replay(response);

            var universalResponse = new UniversalResponse(response);

            Assert.That(universalResponse.ContentEncoding, Is.EqualTo(response.ContentEncoding));
            Assert.That(universalResponse.ContentType, Is.EqualTo(response.ContentType));
            Assert.That(universalResponse.Headers, Is.EqualTo(response.Headers));
            Assert.That(universalResponse.OutputStream, Is.EqualTo(response.OutputStream));
            Assert.That(universalResponse.RedirectLocation, Is.EqualTo(response.RedirectLocation));
            Assert.That(universalResponse.StatusCode, Is.EqualTo(response.StatusCode));
            Assert.That(universalResponse.StatusDescription, Is.EqualTo(response.StatusDescription));
            Assert.That(universalResponse.OriginalResponse, Is.SameAs(response));

            var httpResponse = new HttpResponse(new StringWriter());
            universalResponse = new UniversalResponse(httpResponse);
            universalResponse.ContentEncoding = Encoding.ASCII;
            Assert.That(httpResponse.ContentEncoding, Is.EqualTo(Encoding.ASCII));
            universalResponse.ContentType = "text/html";
            Assert.That(httpResponse.ContentType, Is.EqualTo("text/html"));
            universalResponse.RedirectLocation = "http://tempuri.org";
            Assert.That(httpResponse.RedirectLocation, Is.EqualTo("http://tempuri.org"));
            universalResponse.StatusCode = 301;
            Assert.That(httpResponse.StatusCode, Is.EqualTo(301));
            universalResponse.StatusDescription = "foobar";
            Assert.That(httpResponse.StatusDescription, Is.EqualTo("foobar"));
        }
    }
}
