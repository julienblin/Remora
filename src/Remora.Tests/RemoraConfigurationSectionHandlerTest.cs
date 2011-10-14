using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using NUnit.Framework;
using Remora.Impl;

namespace Remora.Tests
{
    [TestFixture]
    public class RemoraConfigurationSectionHandlerTest
    {
        [Test]
        public void It_should_load_the_minimal_sample()
        {
            var configSection = (IRemoraConfig)ConfigurationManager.GetSection("minimalSample");

            Assert.That(configSection.RequestProcessorType, Is.EqualTo(typeof(RequestProcessor)));
            Assert.That(configSection.CategoryResolverType, Is.EqualTo(typeof(CategoryResolver)));
            Assert.That(configSection.Categories.Count(), Is.EqualTo(0));
        }

        [Test]
        public void It_should_load_the_override_components()
        {
            var configSection = (IRemoraConfig)ConfigurationManager.GetSection("overrideComponents");

            Assert.That(configSection.RequestProcessorType, Is.EqualTo(typeof(CustomRequestProcessor)));
            Assert.That(configSection.CategoryResolverType, Is.EqualTo(typeof(CustomCategoryResolver)));
            Assert.That(configSection.Categories.Count(), Is.EqualTo(0));
        }

        [Test]
        public void It_should_load_a_complete_sample()
        {
            var configSection = (IRemoraConfig)ConfigurationManager.GetSection("completeSample");

            Assert.That(configSection.RequestProcessorType, Is.EqualTo(typeof(RequestProcessor)));
            Assert.That(configSection.CategoryResolverType, Is.EqualTo(typeof(CategoryResolver)));
            Assert.That(configSection.Categories.Count(), Is.EqualTo(2));

            var defaultCat = (from cat in configSection.Categories
                             where cat.Name == "default"
                             select cat
                           ).FirstOrDefault();

            Assert.That(defaultCat, Is.Not.Null);
            Assert.That(defaultCat.Properties.Count(), Is.EqualTo(0));
            Assert.That(defaultCat.PipelineComponents.Count(), Is.EqualTo(0));

            var sampleCat = (from cat in configSection.Categories
                            where cat.Name == "SampleCategory"
                            select cat
                           ).FirstOrDefault();

            Assert.That(sampleCat, Is.Not.Null);
            Assert.That(sampleCat.Properties.Count(), Is.EqualTo(2));
            Assert.That(sampleCat.Properties["property1"], Is.EqualTo("foo"));
            Assert.That(sampleCat.Properties["property2"], Is.EqualTo("bar"));
            Assert.That(sampleCat.PipelineComponents.Count(), Is.EqualTo(2));

            var firstComponent = sampleCat.PipelineComponents.First();
            Assert.That(firstComponent.Type, Is.EqualTo(typeof(CustomPipelineComponent)));
            Assert.That(firstComponent.Properties["property3"], Is.EqualTo("foo"));
            Assert.That(firstComponent.Properties["property4"], Is.EqualTo("bar"));

            var secondComponent = sampleCat.PipelineComponents.Skip(1).First();
            Assert.That(secondComponent.Type, Is.EqualTo(typeof(CustomPipelineComponent)));
            Assert.That(secondComponent.Properties["property5"], Is.EqualTo("foo"));
            Assert.That(secondComponent.Properties["property6"], Is.EqualTo("bar"));
        }
    }

    public class CustomRequestProcessor : IRequestProcessor
    {
        public void Process(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomCategoryResolver : ICategoryResolver
    {
        public Category Resolve(string url)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomPipelineComponent : AbstractPipelineComponent
    {
        public override void Proceed(IPipelineComponentInvocation nextInvocation)
        {
            throw new NotImplementedException();
        }
    }
}
