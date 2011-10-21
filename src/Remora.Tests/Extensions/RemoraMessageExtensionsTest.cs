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
using System.Text;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class RemoraMessageExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_read_data_using_encoding()
        {
            var dataRef = @"foobar";
            var encodingRef = Encoding.GetEncoding("IBM00858");

            var message = new RemoraRequest
                              {
                                  ContentEncoding = encodingRef,
                                  Data = encodingRef.GetBytes(dataRef)
                              };

            Assert.That(message.GetDataAsString(), Is.EqualTo(dataRef));
        }

        [Test]
        public void It_should_set_data_using_encoding()
        {
            var dataRef = @"foobar";
            var encodingRef = Encoding.GetEncoding("IBM00858");

            var message = new RemoraRequest
                              {
                                  ContentEncoding = encodingRef
                              };

            message.SetData(dataRef);

            Assert.That(message.Data, Is.EqualTo(encodingRef.GetBytes(dataRef)));
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => RemoraMessageExtensions.GetDataAsString(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("message"));

            Assert.That(() => RemoraMessageExtensions.SetData(null, null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("message"));

            Assert.That(() => new RemoraRequest().SetData(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("data"));
        }
    }
}