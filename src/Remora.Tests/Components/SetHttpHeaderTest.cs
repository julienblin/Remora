using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Components;
using Remora.Configuration.Impl;
using Remora.Core.Impl;
using Remora.Exceptions;

namespace Remora.Tests.Components
{
    [TestFixture]
    public class SetHttpHeaderTest : BaseTest
    {
        private SetHttpHeader _setHttpHeader;

        [SetUp]
        public void SetUp()
        {
            _setHttpHeader = new SetHttpHeader { Logger = GetConsoleLogger() };
        }

        [Test]
        public void It_should_throw_an_exception_if_name_or_value_not_in_component_definition()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri(@"http://tempuri.org") };
            var componentDefinition = new ComponentDefinition();

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                Throws.Exception.TypeOf<SetHttpHeaderException>()
                .With.Message.Contains("name"));

            componentDefinition.Properties["name"] = "foo";

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                Throws.Exception.TypeOf<SetHttpHeaderException>()
                .With.Message.Contains("value"));

            componentDefinition.Properties["name"] = "foo";
            componentDefinition.Properties["value"] = "bar";

            Assert.That(() => _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b => { }),
                Throws.Nothing);
        }

        [Test]
        public void It_should_position_http_headers()
        {
            var operation = new RemoraOperation { IncomingUri = new Uri(@"http://tempuri.org") };
            var componentDefinition = new ComponentDefinition
                                          {
                                              Properties =
                                                  {
                                                      {"name", "foo"},
                                                      {"value", "bar"}
                                                  }
                                          };
            _setHttpHeader.BeginAsyncProcess(operation, componentDefinition, b =>
            {
                Assert.That(b);
                Assert.That(!operation.OnError);
                Assert.That(operation.Request.HttpHeaders["foo"], Is.EqualTo("bar"));                                                      
            });
        }
    }
}
