using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Configuration;

namespace Remora.Tests.Configuration
{
    [TestFixture]
    public class RemoraConfigurationSectionHandlerTest : BaseTest
    {
        [Test]
        public void It_should_load_configuration_section()
        {
            var result = RemoraConfigurationSectionHandler.GetConfiguration();

            Assert.That(result.MaxMessageSize, Is.EqualTo(10));
            Assert.That(result.Properties.Count(), Is.EqualTo(1));
            Assert.That(result.Properties["bar"], Is.EqualTo("foo"));

            Assert.That(result.PipelineDefinitions.Count(), Is.EqualTo(2));

            var firstPipeline = result.PipelineDefinitions.First();
            Assert.That(firstPipeline.Id, Is.EqualTo("simpleone"));
            Assert.That(firstPipeline.UriFilterRegex, Is.EqualTo("/foo/(.*)"));
            Assert.That(firstPipeline.UriRewriteRegex, Is.EqualTo("http://tempuri.org/$1"));
            Assert.That(firstPipeline.ComponentDefinitions.Count(), Is.EqualTo(0));
            Assert.That(firstPipeline.Properties.Count(), Is.EqualTo(1));
            Assert.That(firstPipeline.Properties["addProp"], Is.EqualTo("addValue"));

            var secondPipeline = result.PipelineDefinitions.Skip(1).First();
            Assert.That(secondPipeline.Id, Is.EqualTo("anotherone"));
            Assert.That(secondPipeline.UriFilterRegex, Is.EqualTo("/bar/(.*)"));
            Assert.That(secondPipeline.UriRewriteRegex, Is.EqualTo("http://tempuri.org/$1"));
            Assert.That(secondPipeline.ComponentDefinitions.Count(), Is.EqualTo(2));
            Assert.That(secondPipeline.ComponentDefinitions.First().RefId, Is.EqualTo("testcomponentone"));
            Assert.That(secondPipeline.ComponentDefinitions.First().Properties.Count(), Is.EqualTo(1));
            Assert.That(secondPipeline.ComponentDefinitions.First().Properties["foo"], Is.EqualTo("bar"));
            Assert.That(secondPipeline.ComponentDefinitions.Skip(1).First().RefId, Is.EqualTo("testcomponenttwo"));
        }
    }
}
