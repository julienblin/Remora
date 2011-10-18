using System;
using System.IO;
using System.Web;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Handler.Impl;
using Rhino.Mocks;

namespace Remora.Tests.Handler.Impl
{
    [TestFixture]
    public class ResponseWriterTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _exceptionFormatter = _mocks.StrictMock<IExceptionFormatter>();
        }

        #endregion

        private MockRepository _mocks;
        private IExceptionFormatter _exceptionFormatter;

        [Test]
        public void It_should_use_the_exception_formatter_in_case_of_error()
        {
            var operation = new RemoraOperation { Exception = new Exception() };
            var responseWriter = new ResponseWriter(_exceptionFormatter) { Logger = GetConsoleLogger() };

            using (var writer = new StringWriter())
            {
                var response = new HttpResponse(writer);
                With.Mocks(_mocks).Expecting(() =>
                {
                    _exceptionFormatter.WriteException(operation, response);    
                }).Verify(() =>
                {
                    responseWriter.Write(operation, response);
                });
            }
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => new ResponseWriter(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("exceptionFormatter")
                );

            var responseWriter = new ResponseWriter(_exceptionFormatter) { Logger = GetConsoleLogger() };

            Assert.That(() => responseWriter.Write(null, null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation")
                );

            Assert.That(() => responseWriter.Write(new RemoraOperation(), null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("response")
                );
        }
    }
}
