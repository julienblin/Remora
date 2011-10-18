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

using System.IO;
using NUnit.Framework;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class StreamExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_correctly_read_streams()
        {
            const int sampleDataSize = 90000;
            var sampleData = new byte[sampleDataSize];
            for (var i = 0; i < sampleDataSize; i++)
            {
                sampleData[i] = 5;
            }

            var testStream = new MemoryStream(sampleData);

            var result = testStream.ReadFully(0);

            Assert.That(result, Is.EqualTo(sampleData));
        }

        [Test]
        public void It_should_throw_MaxMessageSizeException_if_limit_is_reached()
        {
            const int sampleDataSize = 90000;
            var sampleData = new byte[sampleDataSize];
            for (var i = 0; i < sampleDataSize; i++)
            {
                sampleData[i] = 5;
            }

            var testStream = new MemoryStream(sampleData);

            Assert.That(() => testStream.ReadFully(10), 
                Throws.Exception.TypeOf<MaxMessageSizeException>()
                .With.Message.Contains("10")
            );
        }
    }
}
