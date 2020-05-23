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
        private readonly string empty = new Uri("/", UriKind.Relative).OriginalString;
        private readonly string foo = new Uri("/foo", UriKind.Relative).OriginalString;
        private readonly string foobar = new Uri("/foo/bar", UriKind.Relative).OriginalString;

        private INonIdempotencySignsCache cache;
        private BlackListIdempotencyResolver blackListIdempotencyResolver;

        [SetUp]
        public void SetUp()
        {
            cache = Substitute.For<INonIdempotencySignsCache>();
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
            MockCache(new NonIdempotencySign(POST, "/foo/*"), new NonIdempotencySign(POST, "/foo"));

            blackListIdempotencyResolver.IsIdempotent(POST, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_work_with_empty_signs()
        {
            MockCache();

            blackListIdempotencyResolver.IsIdempotent(GET, empty).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "/foo1").Should().BeTrue();
        }

        [Test]
        public void Everything_is_not_idempotent()
        {
            MockCache(
                new NonIdempotencySign(POST, "*"),
                new NonIdempotencySign(GET, "*"),
                new NonIdempotencySign(PATCH, "*"));

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
            MockCache(new NonIdempotencySign(method, pathPattern));
        }

        private void MockCache(params NonIdempotencySign[] signs)
        {
            cache.Get().Returns(signs.ToList());
        }
    }
}