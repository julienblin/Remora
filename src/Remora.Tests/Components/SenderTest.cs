﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, (c) => { }),
                Throws.Exception.TypeOf<UnknownDestinationException>()
            );
        }

        [Test]
        public void It_should_throw_a_InvalidDestinationUriException_if_scheme_is_not_http()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("ftp://localhost");

            var sender = new Sender() { Logger = GetConsoleLogger() };

            Assert.That(() => sender.BeginAsyncProcess(operation, (c) => { }),
                Throws.Exception.TypeOf<InvalidDestinationUriException>()
                .With.Message.Contains("http")
            );
        }

        [Test]
        public void It_should_position_an_operation_SendException_if_server_not_ready()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri("http://tempuri.org") };
            operation.Request.Uri = new Uri("http://zxsdfsafdd");

            var sender = new Sender() { Logger = GetConsoleLogger() };

            var ended = false;
            sender.BeginAsyncProcess(operation, (c) =>
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
                    var request = context.Request;

                    Assert.That(request.Headers["foo"], Is.EqualTo("bar"));
                    Assert.That(request.InputStream.ReadFully(), Is.EqualTo(operation.Request.Data));

                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.AddHeader("anotherfoo", "anotherbar");
                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                }, listener);

                var sender = new Sender() { Logger = GetConsoleLogger() };

                var ended = false;
                sender.BeginAsyncProcess(operation, (c) =>
                {
                    Assert.That(operation.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                    Assert.That(operation.Response.HttpHeaders["anotherfoo"], Is.EqualTo("anotherbar"));
                    Assert.That(operation.Response.Data, Is.EqualTo(responseBuffer));
                    ended = true;
                });

                while (!ended) { Thread.Sleep(10); }
            }
        }

    }
}