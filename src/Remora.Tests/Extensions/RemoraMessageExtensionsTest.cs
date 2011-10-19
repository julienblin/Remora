using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Core;
using Remora.Core.Impl;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class RemoraMessageExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            Assert.That(() => RemoraMessageExtensions.GetDataAsString(null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                        .With.Message.Contains("message"));

            Assert.That(() => RemoraMessageExtensions.SetData(null, null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                        .With.Message.Contains("message"));

            Assert.That(() => RemoraMessageExtensions.SetData(new RemoraRequest(), null),
                        Throws.Exception.TypeOf<ArgumentNullException>()
                        .With.Message.Contains("data"));
        }

        [Test]
        public void It_should_read_data_using_encoding()
        {
            var dataRef = @"foobar";
            var encodingRef = Encoding.GetEncoding("IBM00858");

            var message = new RemoraRequest
            {
                ContentEncoding = encodingRef,
                Data = encodingRef.GetBytes(dataRef)
            };

            Assert.That(message.GetDataAsString(), Is.EqualTo(dataRef));
        }

        [Test]
        public void It_should_set_data_using_encoding()
        {
            var dataRef = @"foobar";
            var encodingRef = Encoding.GetEncoding("IBM00858");

            var message = new RemoraRequest
            {
                ContentEncoding = encodingRef
            };

            message.SetData(dataRef);

            Assert.That(message.Data, Is.EqualTo(encodingRef.GetBytes(dataRef)));
        }
    }
}
