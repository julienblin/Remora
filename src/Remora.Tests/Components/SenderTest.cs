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
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration.Impl;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Extensions;
using Remora.Pipeline;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SenderTest : BaseTest
    {
        [Test]
        public void It_should_be_able_to_work_with_reserved_headers()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("http://localhost:8081/foo/");
            operation.Request.HttpHeaders.Add("accept", "image/*");
            operation.Request.HttpHeaders.Add("connection", "foo");
            operation.Request.HttpHeaders.Add("content-length", "foo");
            operation.Request.HttpHeaders.Add("content-type", "text/html");
            operation.Request.HttpHeaders.Add("expect", "foo");
            operation.Request.HttpHeaders.Add("date", "2011-01-01");
            operation.Request.HttpHeaders.Add("host", "localhost");
            operation.Request.HttpHeaders.Add("if-modified-since", "2011-01-01");
            operation.Request.HttpHeaders.Add("range", "foo");
            operation.Request.HttpHeaders.Add("referer", "localhost");
            operation.Request.HttpHeaders.Add("transfer-encoding", "utf-8");
            operation.Request.HttpHeaders.Add("user-agent", "Remora");

            var responseBuffer = Encoding.UTF8.GetBytes("theresponse");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8081/foo/");
                listener.Start();

                listener.BeginGetContext((result) =>
                                             {
                                                 var l = (HttpListener)result.AsyncState;
                                                 var context = l.EndGetContext(result);
                                                 var request = context.Request;

                                                 var response = context.Response;
                                                 response.StatusCode = (int)HttpStatusCode.OK;
                                                 response.OutputStream.Close();
                                             }, listener);

                var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

                var ended = false;
                sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
                                                        {
                                                            ended = true;
                                                        });

                while (!ended) { Thread.Sleep(10); }
            }
        }

        [Test]
        public void It_should_position_an_operation_SendException_if_server_not_ready()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org")};
            operation.Request.Uri = new Uri("http://zxsdfsafdd");

            var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

            var ended = false;
            sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
            {
                Assert.That(operation.OnError);
                Assert.That(operation.Exception, Is.TypeOf<SendException>());
                ended = true;
            });

            while(!ended) { Thread.Sleep(10); }
        }

        [Test]
        public void It_should_send_and_get_response()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org")};
            operation.Request.Uri = new Uri("http://localhost:8081/foo/");
            operation.Request.HttpHeaders.Add("foo", "bar");
            operation.Request.Data = Encoding.UTF8.GetBytes("bonjour");

            var responseBuffer = Encoding.UTF8.GetBytes("theresponse");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8081/foo/");
                listener.Start();

                listener.BeginGetContext((result) =>
                {
                    var l = (HttpListener)result.AsyncState;
                    var context = l.EndGetContext(result);
                    var request = context.Request;

                    Assert.That(request.Headers["foo"], Is.EqualTo("bar"));
                    Assert.That(request.InputStream.ReadFully(0), Is.EqualTo(operation.Request.Data));

                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.AddHeader("anotherfoo", "anotherbar");
                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

                var ended = false;
                sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
                {
                    Assert.That(operation.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                    Assert.That(operation.Response.HttpHeaders["anotherfoo"], Is.EqualTo("anotherbar"));
                    Assert.That(operation.Response.Data, Is.EqualTo(responseBuffer));
                    Assert.That(!operation.OnError);
                    ended = true;
                });

                while (!ended) { Thread.Sleep(10); }
            }
        }

        [Test]
        public void It_should_use_force_response_encoding_if_defined()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("http://localhost:8081/foo/");
            operation.Request.HttpHeaders.Add("foo", "bar");
            operation.Request.Data = Encoding.UTF8.GetBytes("bonjour");

            var pipelineDefinition = new PipelineDefinition
            {
                Properties = { { "forceResponseEncoding", "ibm861" } }
            };
            operation.ExecutingPipeline = new Remora.Pipeline.Impl.Pipeline("default", new IPipelineComponent[0], pipelineDefinition);

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8081/foo/");
                listener.Start();

                listener.BeginGetContext((result) =>
                {
                    var l = (HttpListener)result.AsyncState;
                    var context = l.EndGetContext(result);
                    var response = context.Response;
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

                var ended = false;
                sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
                {
                    Assert.That(!operation.OnError);
                    Assert.That(operation.Response.ContentEncoding, Is.EqualTo(Encoding.GetEncoding("ibm861")));
                    ended = true;
                });

                while (!ended) { Thread.Sleep(10); }
            }
        }

        [Test]
        public void It_should_use_CharacterSet_to_define_encoding()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("http://localhost:8081/foo/");
            operation.Request.HttpHeaders.Add("foo", "bar");
            operation.Request.Data = Encoding.UTF8.GetBytes("bonjour");

            var responseBuffer = Encoding.UTF8.GetBytes("theresponse");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8081/foo/");
                listener.Start();

                listener.BeginGetContext((result) =>
                {
                    var l = (HttpListener)result.AsyncState;
                    var context = l.EndGetContext(result);
                    var response = context.Response;
                    response.ContentType = string.Format("text/html; charset={0}", Encoding.GetEncoding("IBM01143").HeaderName);

                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

                var ended = false;
                sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
                {
                    Assert.That(!operation.OnError);
                    Assert.That(operation.Response.ContentEncoding, Is.EqualTo(Encoding.GetEncoding("IBM01143")));
                    ended = true;
                });

                while (!ended) { Thread.Sleep(10); }
            }
        }

        [Test]
        public void It_should_throw_a_InvalidDestinationUriException_if_scheme_is_not_http()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org")};
            operation.Request.Uri = new Uri("ftp://localhost");

            var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) => { }),
                        Throws.Exception.TypeOf<InvalidDestinationUriException>()
                            .With.Message.Contains("http")
                );
        }

        [Test]
        public void It_should_throw_a_UnknownDestinationException_if_no_destination_uri()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org")};

            var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) => { }),
                        Throws.Exception.TypeOf<UnknownDestinationException>()
                );
        }

        [Test]
        public void It_should_throw_a_ClientCertificateException_when_client_certificate_does_not_exists()
        {
            var operation = new RemoraOperation
                                {
                                    IncomingUri = new Uri("http://tempuri.org"),
                                    Request =  { Uri = new Uri("http://tempuri.org") },
                                    ExecutingPipeline = new Remora.Pipeline.Impl.Pipeline("default", null, new PipelineDefinition { ClientCertificateFilePath = @"C:\unknown.pfx" })
                                };
            
            var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) => { }),
                        Throws.Exception.TypeOf<ClientCertificateException>()
                        .With.Message.Contains(@"C:\unknown.pfx")
                );
        }

        [Test]
        public void It_should_throw_a_ClientCertificateException_when_password_is_wrong()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri("http://tempuri.org"),
                Request = { Uri = new Uri("http://tempuri.org") },
                ExecutingPipeline = new Remora.Pipeline.Impl.Pipeline("default", null, new PipelineDefinition { ClientCertificateFilePath = @"Certificates\Remora.Tests.pfx", ClientCertificatePassword = @"wrong"})
            };

            var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) => { }),
                        Throws.Exception.TypeOf<ClientCertificateException>()
                        .With.Message.Contains(operation.ExecutingPipeline.Definition.ClientCertificateFilePath)
                );
        }

        [Test]
        public void It_should_send_with_a_client_certificate()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("http://localhost:8081/foo/");
            operation.ExecutingPipeline = new Remora.Pipeline.Impl.Pipeline("default", null, new PipelineDefinition { ClientCertificateFilePath = @"Certificates\Remora.Tests.pfx", ClientCertificatePassword = @"Remora" });
            operation.Request.Data = Encoding.UTF8.GetBytes("bonjour");

            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8081/foo/");
                listener.Start();

                listener.BeginGetContext((result) =>
                {
                    var l = (HttpListener)result.AsyncState;
                    var context = l.EndGetContext(result);
                    var request = context.Request;

                    // Don't want to unit test with Server certificates ;-)

                    var response = context.Response;
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender(new RemoraConfig()) { Logger = GetConsoleLogger() };

                var ended = false;
                Assert.That(() => sender.BeginAsyncProcess(operation, new ComponentDefinition(), (c) =>
                {
                    ended = true;
                }), Throws.Nothing);

                while (!ended) { Thread.Sleep(10); }
            }
        }


        [Test]
        public void It_should_validate_inputs()
        {
            Assert.That(() => new Sender(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                            .With.Message.Contains("config"));
        }
    }
}
