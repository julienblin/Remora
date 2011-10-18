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

using System;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;

namespace Remora.Tests.Core.Impl
{
    [TestFixture]
    public class RemoraOperationKindIdentifierTest : BaseTest
    {
        [Test]
        public void It_should_identify_soap_requests_by_SOAPAction_header()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };
            var operation = new RemoraOperation();
            operation.Request.HttpHeaders.Add("SOAPAction", "http://tempuri.org/");

            Assert.That(identifier.Identify(operation), Is.EqualTo(RemoraOperationKind.Soap));
        }

        [Test]
        public void It_should_return_unknown()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };
            var operation = new RemoraOperation();

            Assert.That(identifier.Identify(operation), Is.EqualTo(RemoraOperationKind.Unknown));
        }

        [Test]
        public void It_should_validate_arguments()
        {
            var identifier = new RemoraOperationKindIdentifier { Logger = GetConsoleLogger() };

            Assert.That(() => identifier.Identify(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("operation"));
        }
    }
}
