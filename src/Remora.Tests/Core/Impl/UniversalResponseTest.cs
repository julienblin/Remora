#region Licence

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

using System.IO;
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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
        }

        #endregion

        private MockRepository _mocks;

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