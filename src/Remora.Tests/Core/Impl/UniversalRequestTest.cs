using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using NUnit.Framework;
using Remora.Core.Impl;
using Rhino.Mocks;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class UniversalRequestTest : BaseTest
    {
        private MockRepository _mocks;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        [Test]
        public void It_should_work_with_HttpRequest()
        {
            var request = _mocks.Stub<HttpRequestBase>();
            _mocks.Replay(request);

            var universalRequest = new UniversalRequest(request);

            Assert.That(universalRequest.AcceptTypes, Is.EqualTo(request.AcceptTypes));
            Assert.That(universalRequest.ContentEncoding, Is.EqualTo(request.ContentEncoding));
            Assert.That(universalRequest.ContentLength, Is.EqualTo(request.ContentLength));
            Assert.That(universalRequest.ContentType, Is.EqualTo(request.ContentType));
            Assert.That(universalRequest.Cookies, Is.EqualTo(request.Cookies));
            Assert.That(universalRequest.Headers, Is.EqualTo(request.Headers));
            Assert.That(universalRequest.HttpMethod, Is.EqualTo(request.HttpMethod));
            Assert.That(universalRequest.InputStream, Is.EqualTo(request.InputStream));
            Assert.That(universalRequest.IsAuthenticated, Is.EqualTo(request.IsAuthenticated));
            Assert.That(universalRequest.IsLocal, Is.EqualTo(request.IsLocal));
            Assert.That(universalRequest.IsSecureConnection, Is.EqualTo(request.IsSecureConnection));
            Assert.That(universalRequest.QueryString, Is.EqualTo(request.QueryString));
            Assert.That(universalRequest.RawUrl, Is.EqualTo(request.RawUrl));
            Assert.That(universalRequest.Url, Is.EqualTo(request.Url));
            Assert.That(universalRequest.UrlReferrer, Is.EqualTo(request.UrlReferrer));
            Assert.That(universalRequest.UserAgent, Is.EqualTo(request.UserAgent));
            Assert.That(universalRequest.UserHostAddress, Is.EqualTo(request.UserHostAddress));
            Assert.That(universalRequest.UserHostName, Is.EqualTo(request.UserHostName));
            Assert.That(universalRequest.UserLanguages, Is.EqualTo(request.UserLanguages));
            Assert.That(universalRequest.OriginalRequest, Is.SameAs(request));


            request = _mocks.DynamicMock<HttpRequestBase>();
            SetupResult.For(request.AcceptTypes).Return(new[] { "foo" });
            SetupResult.For(request.ContentEncoding).Return(Encoding.ASCII);
            SetupResult.For(request.ContentLength).Return(100);
            SetupResult.For(request.ContentType).Return("text/html");
            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie("cookieName", "cookieValue"));
            SetupResult.For(request.Cookies).Return(cookies);
            SetupResult.For(request.Headers).Return(new NameValueCollection { {"headerName", "headerValue"} });
            SetupResult.For(request.HttpMethod).Return("GET");
            SetupResult.For(request.InputStream).Return(new MemoryStream());
            SetupResult.For(request.IsAuthenticated).Return(true);
            SetupResult.For(request.IsLocal).Return(true);
            SetupResult.For(request.IsSecureConnection).Return(true);
            SetupResult.For(request.QueryString).Return(new NameValueCollection { { "queryStringName", "queryStringValue" } });
            SetupResult.For(request.RawUrl).Return("http://tempuri.org");
            SetupResult.For(request.Url).Return(new Uri("http://tempuri.org"));
            SetupResult.For(request.UrlReferrer).Return(new Uri("http://tempuri.org/referrer"));
            SetupResult.For(request.UserAgent).Return("UserAgent");
            SetupResult.For(request.UserHostAddress).Return("UserHostAddress");
            SetupResult.For(request.UserHostName).Return("UserHostName");
            SetupResult.For(request.UserLanguages).Return(new[] { "fr", "en" });

            _mocks.Replay(request);

            universalRequest = new UniversalRequest(request);
            Assert.That(universalRequest.AcceptTypes, Is.EqualTo(request.AcceptTypes));
            Assert.That(universalRequest.ContentEncoding, Is.EqualTo(request.ContentEncoding));
            Assert.That(universalRequest.ContentLength, Is.EqualTo(request.ContentLength));
            Assert.That(universalRequest.ContentType, Is.EqualTo(request.ContentType));
            Assert.That(universalRequest.Cookies.Count, Is.EqualTo(1));
            Assert.That(universalRequest.Cookies.First().Key, Is.EqualTo("cookieName"));
            Assert.That(universalRequest.Cookies.First().Value, Is.EqualTo("cookieValue"));
            Assert.That(universalRequest.Headers.Count, Is.EqualTo(1));
            Assert.That(universalRequest.Headers.First().Key, Is.EqualTo("headerName"));
            Assert.That(universalRequest.Headers.First().Value, Is.EqualTo("headerValue"));
            Assert.That(universalRequest.HttpMethod, Is.EqualTo(request.HttpMethod));
            Assert.That(universalRequest.InputStream, Is.EqualTo(request.InputStream));
            Assert.That(universalRequest.IsAuthenticated, Is.EqualTo(request.IsAuthenticated));
            Assert.That(universalRequest.IsLocal, Is.EqualTo(request.IsLocal));
            Assert.That(universalRequest.IsSecureConnection, Is.EqualTo(request.IsSecureConnection));
            Assert.That(universalRequest.QueryString.Count, Is.EqualTo(1));
            Assert.That(universalRequest.QueryString.First().Key, Is.EqualTo("queryStringName"));
            Assert.That(universalRequest.QueryString.First().Value, Is.EqualTo("queryStringValue"));
            Assert.That(universalRequest.RawUrl, Is.EqualTo(request.RawUrl));
            Assert.That(universalRequest.Url, Is.EqualTo(request.Url));
            Assert.That(universalRequest.UrlReferrer, Is.EqualTo(request.UrlReferrer));
            Assert.That(universalRequest.UserAgent, Is.EqualTo(request.UserAgent));
            Assert.That(universalRequest.UserHostAddress, Is.EqualTo(request.UserHostAddress));
            Assert.That(universalRequest.UserHostName, Is.EqualTo(request.UserHostName));
            Assert.That(universalRequest.UserLanguages, Is.EqualTo(request.UserLanguages));
        }
    }
}
