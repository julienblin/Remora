﻿#region Licence

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
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Core.Serialization;

namespace Remora.Tests.Core.Serialization
{
    [TestFixture]
    public class SerializableOperationTest : BaseTest
    {
        [Test]
        public void It_should_serialize_an_operation()
        {
            var refDataRequest = Encoding.ASCII.GetBytes("dataRequest");
            var refDataResponse = Encoding.UTF7.GetBytes("dataResponse");

            var operation = new RemoraOperation
                                {
                                    IncomingUri = new Uri("http://tempuri.org/incoming"),
                                    Request =
                                        {
                                            ContentEncoding = Encoding.ASCII,
                                            Data = refDataRequest,
                                            HttpHeaders = {{"foo", "bar"}},
                                            Method = "GET",
                                            Uri = new Uri("http://tempuri.org/request")
                                        },
                                    Response =
                                        {
                                            ContentEncoding = Encoding.UTF7,
                                            Data = refDataResponse,
                                            HttpHeaders = {{"bar", "foo"}},
                                            StatusCode = (int) HttpStatusCode.SeeOther,
                                            Uri = new Uri("http://tempuri.org/response")
                                        },
                                    Kind = RemoraOperationKind.Soap
                                };

            var serializableOperation = new SerializableOperation(operation);

            using (var stream = new MemoryStream())
            {
                serializableOperation.Serialize(stream);
                stream.Position = 0;
                var deserializableOperation = SerializableOperation.Deserialize(stream);
                Assert.That(deserializableOperation.IncomingUri, Is.EqualTo(operation.IncomingUri.ToString()));

                Assert.That(deserializableOperation.Request.ContentEncoding,
                            Is.EqualTo(operation.Request.ContentEncoding.HeaderName));
                Assert.That(deserializableOperation.Request.GetData(), Is.EqualTo(operation.Request.Data));
                Assert.That(deserializableOperation.Request.Headers.Count(), Is.EqualTo(1));
                Assert.That(deserializableOperation.Request.Method, Is.EqualTo(operation.Request.Method));
                Assert.That(deserializableOperation.Request.Uri, Is.EqualTo(operation.Request.Uri.ToString()));

                Assert.That(deserializableOperation.Response.ContentEncoding,
                            Is.EqualTo(operation.Response.ContentEncoding.HeaderName));
                Assert.That(deserializableOperation.Response.GetData(), Is.EqualTo(operation.Response.Data));
                Assert.That(deserializableOperation.Response.Headers.Count(), Is.EqualTo(1));
                Assert.That(deserializableOperation.Response.StatusCode, Is.EqualTo(operation.Response.StatusCode));
                Assert.That(deserializableOperation.Response.Uri, Is.EqualTo(operation.Response.Uri.ToString()));
            }
        }
    }
}