using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Extensions;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SenderTest : BaseTest
    {
        [Test]
        public void It_should_throw_a_UnknownDestinationException_if_no_destination_uri()
        {
            var invocation = new TestPipelineComponentInvocation();
            invocation.Operation.IncomingRequest.Uri = "http://localhost";

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.Proceed(invocation),
                Throws.Exception.TypeOf<UnknownDestinationException>()
            );
        }

        [Test]
        public void It_should_throw_a_InvalidDestinationUriException_if_invalid_destination_uri()
        {
            var invocation = new TestPipelineComponentInvocation();
            invocation.Operation.IncomingRequest.Uri = "http://localhost";
            invocation.Operation.OutgoingRequest.Uri = "foo";

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.Proceed(invocation),
                Throws.Exception.TypeOf<InvalidDestinationUriException>()
                .With.Message.Contains(invocation.Operation.OutgoingRequest.Uri)
            );
        }

        [Test]
        public void It_should_throw_a_InvalidDestinationUriException_if_scheme_is_not_http()
        {
            var invocation = new TestPipelineComponentInvocation();
            invocation.Operation.IncomingRequest.Uri = "http://localhost";
            invocation.Operation.OutgoingRequest.Uri = "ftp://localhost";

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.Proceed(invocation),
                Throws.Exception.TypeOf<InvalidDestinationUriException>()
                .With.Message.Contains("http")
            );
        }

        [Test]
        public void It_should_throw_a_SendException_if_server_not_ready()
        {
            var invocation = new TestPipelineComponentInvocation();
            invocation.Operation.IncomingRequest.Uri = "http://localhost";
            invocation.Operation.OutgoingRequest.Uri = "http://asdfasfdasdfsad";

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.Proceed(invocation),
                Throws.Exception.TypeOf<SendException>()
            );
        }

        [Test]
        public void It_should_send_and_get_response()
        {
            var invocation = new TestPipelineComponentInvocation();
            invocation.Operation.IncomingRequest.Uri = "http://localhost";
            invocation.Operation.OutgoingRequest.Uri = "http://localhost:8081/foo/";
            invocation.Operation.OutgoingRequest.HttpHeaders.Add("foo", "bar");
            invocation.Operation.OutgoingRequest.Data = Encoding.UTF8.GetBytes("bonjour");

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
                    Assert.That(request.InputStream.ReadFully(), Is.EqualTo(invocation.Operation.OutgoingRequest.Data));

                    var response = context.Response;
                    response.StatusCode = (int) HttpStatusCode.OK;
                    response.AddHeader("anotherfoo", "anotherbar");
                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender() { Logger = GetConsoleLogger() };

                sender.Proceed(invocation);

                Assert.That(invocation.Operation.IncomingResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(invocation.Operation.OutgoingResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

                Assert.That(invocation.Operation.IncomingResponse.HttpHeaders["anotherfoo"], Is.EqualTo("anotherbar"));
                Assert.That(invocation.Operation.OutgoingResponse.HttpHeaders["anotherfoo"], Is.EqualTo("anotherbar"));

                Assert.That(invocation.Operation.IncomingResponse.Data, Is.EqualTo(responseBuffer));
                Assert.That(invocation.Operation.OutgoingResponse.Data, Is.EqualTo(responseBuffer));
            }
        }
    }
}
