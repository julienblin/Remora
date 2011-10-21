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
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration.Impl;
using Remora.Core.Impl;
using Remora.Exceptions;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SetHttpHeaderTest : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _setHttpHeader = new SetHttpHeader {Logger = GetConsoleLogger()};
        }

        #endregion

        private SetHttpHeader _setHttpHeader;

        [Test]
        public void It_should_position_http_headers()
        {
            var operation = new RemoraOperation {IncomingUri = new Uri(@"http://tempuri.org")};
            var componentDefinition = new ComponentDefinition
                                          {
                                              Properties =
                                                  {
                                                      {"name", "foo"},
                                                      {"value", "bar"}
                                                  }
                                          };
            _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b =>
                                                                                 {
                                                                                     Assert.That(b);
                                                                                     Assert.That(!operation.OnError);
                                                                                     Assert.That(
                                                                                         operation.Request.HttpHeaders[
                                                                                             "foo"], Is.EqualTo("bar"));
                                                                                 });
        }

        [Test]
        public void It_should_throw_an_exception_if_name_or_value_not_in_component_definition()
        {
            var operation = new RemoraOperation {IncomingUri = new Uri(@"http://tempuri.org")};
            var componentDefinition = new ComponentDefinition();

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                        Throws.Exception.TypeOf<SetHttpHeaderException>()
                            .With.Message.Contains("name"));

            componentDefinition.Properties["name"] = "foo";

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                        Throws.Exception.TypeOf<SetHttpHeaderException>()
                            .With.Message.Contains("value"));

            componentDefinition.Properties["name"] = "foo";
            componentDefinition.Properties["value"] = "bar";

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                        Throws.Nothing);
        }
    }
}