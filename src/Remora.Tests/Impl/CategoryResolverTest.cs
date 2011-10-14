using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Impl;
using Rhino.Mocks;

namespace Remora.Tests.Impl
{
    [TestFixture]
    public class CategoryResolverTest
    {
        public MockRepository _mocks;
        public IRemoraConfig _config;
        public CategoryResolver _resolver;

        [SetUp]
        public void SetUp()
        {
            _mocks = new MockRepository();
            _config = _mocks.StrictMock<IRemoraConfig>();
            _resolver = new CategoryResolver(_config);
        }

        [Test]
        public void It_should_ensure_that_it_has_a_IRemoraConfig()
        {
            Assert.That(() => new CategoryResolver(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("config"));
        }

        [Test]
        public void It_should_throw_an_exception_when_url_is_null()
        {
            Assert.That(() => _resolver.Resolve(null),
                Throws.Exception.TypeOf<ArgumentNullException>()
                .With.Message.Contains("url"));
        }

        [Test]
        public void It_should_throw_a_RemoraException_when_no_matching_url_is_found()
        {
            With.Mocks(_mocks).Expecting(() =>
            {
                var categories = new List<Category>();
                SetupResult.For(_config.Categories).Return(categories);
            }).Verify(() =>
            {
                Assert.That(() => _resolver.Resolve("foo"),
                 Throws.Exception.TypeOf<RemoraException>()
                 .With.Message.Contains("foo"));       
            });
        }

        //[Test]
        //public void It_should_return_a_matching_category()
        //{
        //    var cat1 = new Category { UrlMatcher = "foo.*" };
        //    var cat2 = new Category { UrlMatcher = "bar.*" };
        //    With.Mocks(_mocks).Expecting(() =>
        //    {
        //        var categories = new List<Category> { cat1, cat2 };
        //        SetupResult.For(_config.Categories).Return(categories);
        //    }).Verify(() =>
        //    {
        //        Assert.That(() => _resolver.Resolve("bar"),
        //         Is.SameAs(cat2));
        //    });
        //}

        //[Test]
        //public void It_should_return_the_first_matching_category()
        //{
        //    var cat1 = new Category { UrlMatcher = "fooextended" };
        //    var cat2 = new Category { UrlMatcher = "foo.*" };
        //    With.Mocks(_mocks).Expecting(() =>
        //    {
        //        var categories = new List<Category> { cat1, cat2 };
        //        SetupResult.For(_config.Categories).Return(categories);
        //    }).Verify(() =>
        //    {
        //        Assert.That(() => _resolver.Resolve("fooextended"),
        //         Is.SameAs(cat1));
        //    });
        //}

    }
}
