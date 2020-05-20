using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency.BlackList;
using Vostok.Singular.Core.Idempotency.BlackList.Settings;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    internal class BlackListIdempotencyResolverTests
    {
        private const string POST = "POST";
        private const string GET = "GET";
        private const string PATCH = "PATCH";
        private string empty = new Uri("/", UriKind.Relative).OriginalString;
        private string foo = new Uri("/foo", UriKind.Relative).OriginalString;
        private string foobar = new Uri("/foo/bar", UriKind.Relative).OriginalString;

        private IIdempotencySignsProvider provider;
        private BlackListIdempotencyResolver blackListIdempotencyResolver;

        [SetUp]
        public void SetUp()
        {
            provider = Substitute.For<IIdempotencySignsProvider>();
            var cache = new NonIdempotencySignsCache(provider);
            blackListIdempotencyResolver = new BlackListIdempotencyResolver(cache);
        }

        [Test]
        public void Should_detect_not_idempotent_methods()
        {
            MockCache(POST, "*");

            blackListIdempotencyResolver.IsIdempotent(GET, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(GET, foo).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(GET, foobar).Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path()
        {
            MockCache(POST, "/foo");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern()
        {
            MockCache(POST, "/foo*");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_path_delimiter()
        {
            MockCache(POST, "/foo/*");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_and_without_path_delimiter()
        {
            MockCache(new NonIdempotencySignSettings { Method = POST, PathPattern = "/foo/*" }, new NonIdempotencySignSettings { Method = POST, PathPattern = "/foo" });

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_work_with_empty_signs()
        {
            MockCache(new NonIdempotencySignSettings[0]);

            blackListIdempotencyResolver.IsIdempotent(GET, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "/foo1").Should().BeTrue();
        }

        [Test]
        public void Everything_is_not_idempotent()
        {
            MockCache(
                new NonIdempotencySignSettings { Method = POST, PathPattern = "*" },
                new NonIdempotencySignSettings { Method = GET, PathPattern = "*" },
                new NonIdempotencySignSettings { Method = PATCH, PathPattern = "*" });

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();

            blackListIdempotencyResolver.IsIdempotent(GET, empty).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, "/foo1").Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, foobar).Should().BeFalse();

            blackListIdempotencyResolver.IsIdempotent(PATCH, empty).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "/foo1").Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, foobar).Should().BeFalse();
        }

        private void MockCache(string method, string pathPattern)
        {
            MockCache(new NonIdempotencySignSettings { Method = method, PathPattern = pathPattern });
        }

        private void MockCache(params NonIdempotencySignSettings[] signs)
        {
            provider.Get().Returns(new NonIdempotencySignsSettings() { Signs = signs.ToList() });
        }
    }
}