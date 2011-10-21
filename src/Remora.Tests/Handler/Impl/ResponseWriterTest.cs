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
            var operation = new RemoraOperation {Exception = new Exception()};
            var responseWriter = new ResponseWriter(_exceptionFormatter) {Logger = GetConsoleLogger()};

            using (var writer = new StringWriter())
            {
                var response = new HttpResponse(writer);
                With.Mocks(_mocks).Expecting(() => { _exceptionFormatter.WriteException(operation, response); }).Verify(
                    () => { responseWriter.Write(operation, response); });
            }
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => new ResponseWriter(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("exceptionFormatter")
                );

            var responseWriter = new ResponseWriter(_exceptionFormatter) {Logger = GetConsoleLogger()};

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