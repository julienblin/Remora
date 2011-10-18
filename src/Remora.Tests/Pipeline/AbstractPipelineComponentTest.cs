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

using NUnit.Framework;
using Remora.Configuration.Impl;
using Remora.Core.Impl;
using Remora.Pipeline;

namespace Remora.Tests.Pipeline
{
    [TestFixture]
    public class AbstractPipelineComponentTest : BaseTest
    {
        public class ApcTest : AbstractPipelineComponent {}

        [Test]
        public void It_should_call_callback_on_EndAsyncProcess()
        {
            var apc = new ApcTest();
            var wasCalled = false;
            apc.EndAsyncProcess(new RemoraOperation(), new ComponentDefinition(), () =>
                                                           {
                                                               wasCalled = true;
                                                           });
            Assert.That(wasCalled);
        }

        [Test]
        public void It_should_call_callback_with_true_on_BeginAsyncProcess()
        {
            var apc = new ApcTest();
            var wasCalled = false;
            apc.BeginAsyncProcess(new RemoraOperation(), new ComponentDefinition(), (b) =>
                                                             {
                                                                 Assert.That(b);
                                                                 wasCalled = true;
                                                             });
            Assert.That(wasCalled);
        }
    }
}
