﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration.Impl;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Extensions;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SoapRecorderTest : BaseTest
    {
        private SoapRecorder _soapRecorder;

        [SetUp]
        public void SetUp()
        {
            _soapRecorder = new SoapRecorder { Logger = GetConsoleLogger() };
        }

        [Test]
        public void It_should_ignore_non_soap_operations()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri(@"http://tempuri.org") };
            
            Assert.That(() => _soapRecorder.BeginAsyncProcess(operation, null, b =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
            }), Throws.Nothing);

            Assert.That(() => _soapRecorder.EndAsyncProcess(operation, null, () =>
            {
                Assert.That(!operation.OnError);
            }), Throws.Nothing);
        }

        [Test]
        public void It_should_ignore_when_missing_SOAPAction_header()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri(@"http://tempuri.org"), Kind = RemoraOperationKind.Soap };

            Assert.That(() => _soapRecorder.BeginAsyncProcess(operation, null, b =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
            }), Throws.Nothing);

            Assert.That(() => _soapRecorder.EndAsyncProcess(operation, null, () =>
            {
                Assert.That(!operation.OnError);
            }), Throws.Nothing);
        }

        [Test]
        public void It_should_ignore_when_missing_directory_in_component_definition()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap,
                Request =
                {
                    HttpHeaders = { { "SOAPAction", "http://tempuri.org/IService/Method" } }
                }
            };

            var componentDefinition = new ComponentDefinition();

            Assert.That(() => _soapRecorder.BeginAsyncProcess(operation, componentDefinition, b =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
            }), Throws.Nothing);

            Assert.That(() => _soapRecorder.EndAsyncProcess(operation, componentDefinition, () =>
            {
                Assert.That(!operation.OnError);
            }), Throws.Nothing);
        }

        [Test]
        public void It_should_do_nothing_when_directory_cannot_be_created()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap,
                Request =
                {
                    HttpHeaders = { { "SOAPAction", "http://tempuri.org/IService/Method" } }
                }
            };

            var componentDefinition = new ComponentDefinition
            {
                Properties = { { "directory", string.Join("", Path.GetInvalidPathChars()) } }
            };

            Assert.That(() => _soapRecorder.BeginAsyncProcess(operation, componentDefinition, b =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
            }), Throws.Nothing);

            Assert.That(() => _soapRecorder.EndAsyncProcess(operation, componentDefinition, () =>
            {
                Assert.That(!operation.OnError);
            }), Throws.Nothing);
        }

        [Test]
        public void It_should_record_operations()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

            var componentDefinition = new ComponentDefinition
            {
                Properties = { { "directory", tempPath } }
            };

            try
            {
                var operation1 = new RemoraOperation
                {
                    IncomingUri = new Uri(@"http://tempuri.org"),
                    Kind = RemoraOperationKind.Soap,
                    Request =
                    {
                        HttpHeaders = { { "SOAPAction", "http://tempuri.org/IService/Method" } }
                    }
                };

                Assert.That(() => _soapRecorder.BeginAsyncProcess(operation1, componentDefinition, b =>
                {
                    Assert.That(b);
                    Assert.That(!operation1.OnError);
                }), Throws.Nothing);

                Assert.That(() => _soapRecorder.EndAsyncProcess(operation1, componentDefinition, () =>
                {
                    Assert.That(!operation1.OnError);
                }), Throws.Nothing);

                var operation2 = new RemoraOperation
                {
                    IncomingUri = new Uri(@"http://tempuri.org"),
                    Kind = RemoraOperationKind.Soap,
                    Request =
                    {
                        ContentEncoding = Encoding.UTF8,
                        HttpHeaders = { { "SOAPAction", "http://tempuri.org/IService/Method" } },
                        Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0),
                        Method = "POST",
                        Uri = new Uri("http://tempuri.org/foo")
                    },
                    Response =
                    {
                        ContentEncoding = Encoding.UTF8,
                        Data = LoadSample("SimpleHelloWorldRequest.xml").ReadFully(0),
                        Uri = new Uri("http://tempuri.org/foo"),
                        StatusCode = 200
                    }
                };

                Assert.That(() => _soapRecorder.BeginAsyncProcess(operation2, componentDefinition, b =>
                {
                    Assert.That(b);
                    Assert.That(!operation2.OnError);
                }), Throws.Nothing);

                Assert.That(() => _soapRecorder.EndAsyncProcess(operation2, componentDefinition, () =>
                {
                    Assert.That(!operation2.OnError);
                }), Throws.Nothing);

                Assert.That(Directory.EnumerateFiles(tempPath, "http://tempuri.org/IService/Method".MakeValidFileName() + "*").Count(),
                    Is.EqualTo(2));
            }
            finally
            {
                if(Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
            }
        }
    }
}