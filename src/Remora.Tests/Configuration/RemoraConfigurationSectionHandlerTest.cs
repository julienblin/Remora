#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System.Linq;
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
