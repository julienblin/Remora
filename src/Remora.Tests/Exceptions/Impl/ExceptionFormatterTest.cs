using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using Remora.Core.Impl;
using Remora.Exceptions;
using Remora.Exceptions.Impl;

namespace Remora.Tests.Exceptions.Impl
{
    [TestFixture]
    public class ExceptionFormatterTest : BaseTest
    {
        [Test]
        public void It_should_validate_arguments()
        {
            var formatter = new ExceptionFormatter();

            Assert.That(() => formatter.WriteException(null, new HttpResponse(new StringWriter())),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("operation")
            );

            Assert.That(() => formatter.WriteException(new RemoraOperation(), null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("response")
            );
        }
    }
}
