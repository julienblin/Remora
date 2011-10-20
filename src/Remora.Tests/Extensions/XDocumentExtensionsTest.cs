using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class XDocumentExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_normalize_XDocuments()
        {
            var docOK = XDocument.Load(LoadSample("SampleHelloWorldServiceHelloOK.xml"));
            var docVariant = XDocument.Load(LoadSample("SampleHelloWorldServiceHelloVariantOK.xml"));
            var docDifferent = XDocument.Load(LoadSample("SimpleHelloWorldRequest.xml"));

            Assert.That(!XNode.DeepEquals(docOK, docVariant));
            Assert.That(XNode.DeepEquals(docOK.Normalize(), docVariant.Normalize()));
            Assert.That(!XNode.DeepEquals(docOK.Normalize(), docDifferent.Normalize()));
        }
    }
}
