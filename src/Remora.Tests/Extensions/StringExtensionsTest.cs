using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_make_valid_filenames()
        {
            var refStr = "http://tempuri.org/foo";

            Assert.That(refStr.MakeValidFileName(), Is.EqualTo("http_tempuri.org_foo"));
        }
    }
}
