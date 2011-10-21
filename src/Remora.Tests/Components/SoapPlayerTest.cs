﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration;
using Remora.Configuration.Impl;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Extensions;
using Remora.Transformers;
using Remora.Transformers.Impl;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SoapPlayerTest : BaseTest
    {
        private const string SampleSoapAction = @"http://tempuri.org/IHelloWorldService/Hello";
        private static readonly string[] SampleFiles = new string[] { @"http_tempuri.org_IHelloWorldService_Hello.0.xml", @"http_tempuri.org_IHelloWorldService_Hello.1.xml" };

        private ISoapTransformer _soapTransformer;
        private SoapPlayer _soapPlayer;
        private string _tempPath;
        private IComponentDefinition _componentDefinition;

        [SetUp]
        public void SetUp()
        {
            _soapTransformer = new SoapTransformer();
            _soapPlayer = new SoapPlayer(_soapTransformer) { Logger = GetConsoleLogger() };

            _tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            _componentDefinition = new ComponentDefinition
            {
                Properties = { { "directory", _tempPath } }
            };
        }

        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => new SoapPlayer(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("soapTransformer"));

            Assert.That(() => _soapPlayer.BeginAsyncProcess(null, null, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("operation"));

            Assert.That(() => _soapPlayer.BeginAsyncProcess(new RemoraOperation(), null, null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("componentDefinition"));

            Assert.That(() => _soapPlayer.BeginAsyncProcess(new RemoraOperation(), new ComponentDefinition(), null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("callback"));
        }

        [Test]
        public void It_should_throw_an_exception_if_operation_is_not_kind_of_soap()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org")
            };

            Assert.That(() => _soapPlayer.BeginAsyncProcess(operation, new ComponentDefinition(), (b) => { }),
                Throws.Exception.TypeOf<SoapPlayerException>()
                .With.Message.Contains("soap"));
        }

        [Test]
        public void It_should_throw_an_exception_if_definition_doesnt_have_a_directory()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap
            };

            var componentDefinition = new ComponentDefinition
            {
            };

            Assert.That(() => _soapPlayer.BeginAsyncProcess(operation, componentDefinition, (b) => { }),
                Throws.Exception.TypeOf<SoapPlayerException>()
                .With.Message.Contains("directory"));
        }

        [Test]
        public void It_should_throw_an_exception_if_operation_doesnt_have_a_SOAPAction_header()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap
            };

            Assert.That(() => _soapPlayer.BeginAsyncProcess(operation, _componentDefinition, (b) => { }),
                Throws.Exception.TypeOf<SoapPlayerException>()
                .With.Message.Contains("SOAPAction"));
        }

        [Test]
        public void It_should_throw_an_exception_if_directory_is_invalid()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap,
                Request =
                    {
                        HttpHeaders = { { "SOAPAction", SampleSoapAction } }
                    }
            };

            Assert.That(() => _soapPlayer.BeginAsyncProcess(operation, _componentDefinition, (b) => { }),
                Throws.Exception.TypeOf<SoapPlayerException>()
                .With.Message.Contains(_tempPath));
        }

        [Test]
        public void It_should_throw_an_exception_if_unable_to_find_a_mock()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap,
                Request =
                {
                    HttpHeaders = { { "SOAPAction", SampleSoapAction } },
                    ContentEncoding = Encoding.UTF8,
                    Data = LoadSample("SampleHelloWorldServiceHelloFail.xml").ReadFully(0)
                }
            };

            try
            {
                Directory.CreateDirectory(_tempPath);
                foreach (var sampleFile in SampleFiles)
                {
                    File.Copy(@"Samples\" + sampleFile, Path.Combine(_tempPath, sampleFile));
                }

                Assert.That(() => _soapPlayer.BeginAsyncProcess(operation, _componentDefinition, (b) => { }),
                Throws.Exception.TypeOf<SoapPlayerException>()
                .With.Message.Contains("appropriate mock"));
            }
            finally
            {
                if(Directory.Exists(_tempPath))
                    Directory.Delete(_tempPath, true);
            }
        }

        [Test]
        public void It_should_respond_if_mock_found_comparing_body()
        {
            var operation = new RemoraOperation
            {
                IncomingUri = new Uri(@"http://tempuri.org"),
                Kind = RemoraOperationKind.Soap,
                Request =
                {
                    HttpHeaders = { { "SOAPAction", SampleSoapAction } },
                    ContentEncoding = Encoding.UTF8,
                    Data = LoadSample("SampleHelloWorldServiceHelloOK.xml").ReadFully(0)
                }
            };

            try
            {
                Directory.CreateDirectory(_tempPath);
                foreach (var sampleFile in SampleFiles)
                {
                    File.Copy(@"Samples\" + sampleFile, Path.Combine(_tempPath, sampleFile));
                }

                _soapPlayer.BeginAsyncProcess(operation, _componentDefinition, (b) =>
                {
                    Assert.That(!b);
                    Assert.That(operation.Response.ContentEncoding, Is.EqualTo(Encoding.UTF8));
                    Assert.That(operation.Response.HttpHeaders["Server"], Is.EqualTo("ASP.NET Development Server/10.0.0.0"));
                    Assert.That(operation.Response.StatusCode, Is.EqualTo(200));
                    Assert.That(operation.Response.ContentEncoding.GetString(operation.Response.Data), Contains.Substring("Hello, bar"));
                });
            }
            finally
            {
                if (Directory.Exists(_tempPath))
                    Directory.Delete(_tempPath, true);
            }
        }
    }
}