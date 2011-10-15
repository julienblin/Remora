using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Configuration;

namespace Remora.Tests.Configuration
{
    [TestFixture]
    public class PipelinesConfigurationSectionHandlerTest : BaseTest
    {
        [Test]
        public void It_should_load_configuration_section()
        {
            var result = PipelinesConfigurationSectionHandler.GetConfiguration();

            Assert.That(result.Count(), Is.EqualTo(2));

            var firstPipeline = result.First();
            Assert.That(firstPipeline.Id, Is.EqualTo("simpleone"));
            Assert.That(firstPipeline.UriFilterRegex, Is.EqualTo("/foo/(?.*)"));
            Assert.That(firstPipeline.UriRewriteRegex, Is.EqualTo("http://tempuri.org/{1}"));
            Assert.That(firstPipeline.Components.Count(), Is.EqualTo(0));

            var secondPipeline = result.Skip(1).First();
            Assert.That(secondPipeline.Id, Is.EqualTo("anotherone"));
            Assert.That(secondPipeline.UriFilterRegex, Is.EqualTo("/bar/(?.*)"));
            Assert.That(secondPipeline.UriRewriteRegex, Is.EqualTo("http://tempuri.org/{1}"));
            Assert.That(secondPipeline.Components.Count(), Is.EqualTo(2));
            Assert.That(secondPipeline.Components.First(), Is.EqualTo("testcomponentone"));
            Assert.That(secondPipeline.Components.Skip(1).First(), Is.EqualTo("testcomponenttwo"));
        }
    }
}
