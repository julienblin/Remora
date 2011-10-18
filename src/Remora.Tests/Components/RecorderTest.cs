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
    public class RecorderTest : BaseTest
    {
        [Test]
        public void It_should_validate_the_path_in_component_definition()
        {
            var recorder = new Recorder { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org") } };
            operation.Request.Uri = new Uri("http://localhost");

            Assert.That(() => recorder.BeginAsyncProcess(operation, new ComponentDefinition(), (c) => { }),
                        Throws.Exception.TypeOf<RecorderException>()
                            .With.Message.Contains("path")
                );
        }

        [Test]
        public void It_should_validate_the_path_value()
        {
            var recorder = new Recorder { Logger = GetConsoleLogger() };

            var operation = new RemoraOperation { IncomingRequest = { Uri = new Uri("http://tempuri.org") } };
            operation.Request.Uri = new Uri("http://localhost");

            var cmpDef = new ComponentDefinition { Properties = { { "path", "df:\\unabletocreate" } } };

            Assert.That(() => recorder.BeginAsyncProcess(operation, cmpDef, (c) => { }),
                        Throws.Exception.TypeOf<RecorderException>()
                            .With.Message.Contains(cmpDef.Properties["path"])
                );
        }
    }
}
