using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remora.Tests
{
    [TestFixture]
    public class RemoraAsyncResultTest
    {
        public MockRepository _mocks;
        public IRequestProcessor _requestProcessor;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _requestProcessor = _mocks.StrictMock<IRequestProcessor>();
        }

        [Test]
        public void It_should_invoke_RequestProcessor_asynchronously()
        {
            var callbackCalled = false;
            var httpContext = new HttpContext(new HttpRequest("", "http://foo", ""), new HttpResponse(new StringWriter()));
            var asyncResult = new RemoraAsyncResult((result) => { callbackCalled = true; }, httpContext, null);

            asyncResult.RequestProcessor = _requestProcessor;

            With.Mocks(_mocks).Expecting(() =>
            {
                _requestProcessor.Process(httpContext);
            }).Verify(() =>
            {
                asyncResult.Process();
                Thread.Sleep(100);
                Assert.That(callbackCalled);
                Assert.That(asyncResult.IsCompleted);
                Assert.That(asyncResult.CompletedSynchronously, Is.False);
            });
        }
    }
}
