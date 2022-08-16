using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.PathPatterns;
using Vostok.Singular.Core.PathPatterns.BlackList;

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

        private ISettingsCache<NonIdempotencySign> cache;
        private BlackListIdempotencyResolver blackListIdempotencyResolver;

        [SetUp]
        public void SetUp()
        {
            cache = Substitute.For<ISettingsCache<NonIdempotencySign>>();
            blackListIdempotencyResolver = new BlackListIdempotencyResolver(cache);
        }

        [Test]
        public void Should_detect_not_idempotent_methods()
        {
            MockCache(POST, "*");

            blackListIdempotencyResolver.IsIdempotent(GET, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(GET, foo).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(GET, foobar).GetAwaiter().GetResult().Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path()
        {
            MockCache(POST, "foo");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern()
        {
            MockCache(POST, "foo*");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").GetAwaiter().GetResult().Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_path_delimiter()
        {
            MockCache(POST, "foo/*");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_and_without_path_delimiter()
        {
            MockCache(new NonIdempotencySign(POST, "foo/*"), new NonIdempotencySign(POST, "foo"));

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").GetAwaiter().GetResult().Should().BeTrue();

            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeFalse();
        }
        
        [Test]
        public void Should_detect_not_idempotent_methods_with_given_method_pattern()
        {
            MockCache("*", "foo*");
            
            blackListIdempotencyResolver.IsIdempotent(GET, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, foobar).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent("abcd123", "/foo1").GetAwaiter().GetResult().Should().BeFalse();
        }
        
        [Test]
        public void Should_detect_not_idempotent_methods_with_given_method_pattern_only_if_path_matched()
        {
            MockCache("*", "foo*");

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(GET, "afoo").GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "barfoo").GetAwaiter().GetResult().Should().BeTrue();
        }

        [Test]
        public void Should_work_with_empty_signs()
        {
            MockCache();

            blackListIdempotencyResolver.IsIdempotent(GET, empty).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeTrue();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "/foo1").GetAwaiter().GetResult().Should().BeTrue();
        }

        [Test]
        public void Everything_is_not_idempotent()
        {
            MockCache(
                new NonIdempotencySign(POST, "*"),
                new NonIdempotencySign(GET, "*"),
                new NonIdempotencySign(PATCH, "*"));

            blackListIdempotencyResolver.IsIdempotent(POST, empty).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, "/foo1").GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(POST, foobar).GetAwaiter().GetResult().Should().BeFalse();

            blackListIdempotencyResolver.IsIdempotent(GET, empty).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, "/foo1").GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(GET, foobar).GetAwaiter().GetResult().Should().BeFalse();

            blackListIdempotencyResolver.IsIdempotent(PATCH, empty).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, "/foo1").GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, foo).GetAwaiter().GetResult().Should().BeFalse();
            blackListIdempotencyResolver.IsIdempotent(PATCH, foobar).GetAwaiter().GetResult().Should().BeFalse();
        }

        private void MockCache(string method, string pathPattern)
        {
            MockCache(new NonIdempotencySign(method, pathPattern));
        }

        private void MockCache(params NonIdempotencySign[] signs)
        {
            cache.GetAsync().Returns(signs.ToList());
        }
    }
}