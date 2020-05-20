﻿using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency;
using Vostok.Singular.Core.Idempotency.Identifier;
using Vostok.Singular.Core.Idempotency.Settings;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    internal class IdempotencyIdentifierTests
    {
        private const string POST = "POST";
        private const string GET = "GET";
        private const string PATCH = "PATCH";
        private string empty = new Uri("/", UriKind.Relative).OriginalString;
        private string foo = new Uri("/foo", UriKind.Relative).OriginalString;
        private string foobar = new Uri("/foo/bar", UriKind.Relative).OriginalString;

        private IIdempotencySignsProvider provider;
        private IdempotencyIdentifier identifier;

        [SetUp]
        public void SetUp()
        {
            provider = Substitute.For<IIdempotencySignsProvider>();
            var cache = new IdempotencySignsCache(provider);
            identifier = new IdempotencyIdentifier(cache);
        }

        [Test]
        public void Should_detect_not_idempotent_methods()
        {
            MockCache(POST, "*");

            identifier.IsIdempotent(GET, empty).Should().BeTrue();
            identifier.IsIdempotent(GET, foo).Should().BeTrue();
            identifier.IsIdempotent(GET, foobar).Should().BeTrue();

            identifier.IsIdempotent(POST, empty).Should().BeFalse();
            identifier.IsIdempotent(POST, foo).Should().BeFalse();
            identifier.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path()
        {
            MockCache(POST, "/foo");

            identifier.IsIdempotent(POST, empty).Should().BeTrue();
            identifier.IsIdempotent(POST, foobar).Should().BeTrue();

            identifier.IsIdempotent(POST, foo).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern()
        {
            MockCache(POST, "/foo*");

            identifier.IsIdempotent(POST, empty).Should().BeTrue();

            identifier.IsIdempotent(POST, foo).Should().BeFalse();
            identifier.IsIdempotent(POST, foobar).Should().BeFalse();
            identifier.IsIdempotent(POST, "/foo1").Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_path_delimiter()
        {
            MockCache(POST, "/foo/*");

            identifier.IsIdempotent(POST, empty).Should().BeTrue();
            identifier.IsIdempotent(POST, "/foo1").Should().BeTrue();
            identifier.IsIdempotent(POST, foo).Should().BeTrue();

            identifier.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_detect_not_idempotent_methods_with_given_path_pattern_with_and_without_path_delimiter()
        {
            MockCache(new NonIdempotencySignSettings { Method = POST, PathPattern = "/foo/*" }, new NonIdempotencySignSettings { Method = POST, PathPattern = "/foo" });

            identifier.IsIdempotent(POST, empty).Should().BeTrue();
            identifier.IsIdempotent(POST, "/foo1").Should().BeTrue();

            identifier.IsIdempotent(POST, foo).Should().BeFalse();
            identifier.IsIdempotent(POST, foobar).Should().BeFalse();
        }

        [Test]
        public void Should_work_with_empty_signs()
        {
            MockCache(new NonIdempotencySignSettings[0]);

            identifier.IsIdempotent(GET, empty).Should().BeTrue();
            identifier.IsIdempotent(POST, foo).Should().BeTrue();
            identifier.IsIdempotent(POST, foobar).Should().BeTrue();
            identifier.IsIdempotent(PATCH, "/foo1").Should().BeTrue();
        }

        [Test]
        public void Everything_is_not_idempotent()
        {
            MockCache(
                new NonIdempotencySignSettings { Method = POST, PathPattern = "*" },
                new NonIdempotencySignSettings { Method = GET, PathPattern = "*" },
                new NonIdempotencySignSettings { Method = PATCH, PathPattern = "*" });

            identifier.IsIdempotent(POST, empty).Should().BeFalse();
            identifier.IsIdempotent(POST, "/foo1").Should().BeFalse();
            identifier.IsIdempotent(POST, foo).Should().BeFalse();
            identifier.IsIdempotent(POST, foobar).Should().BeFalse();

            identifier.IsIdempotent(GET, empty).Should().BeFalse();
            identifier.IsIdempotent(GET, "/foo1").Should().BeFalse();
            identifier.IsIdempotent(GET, foo).Should().BeFalse();
            identifier.IsIdempotent(GET, foobar).Should().BeFalse();

            identifier.IsIdempotent(PATCH, empty).Should().BeFalse();
            identifier.IsIdempotent(PATCH, "/foo1").Should().BeFalse();
            identifier.IsIdempotent(PATCH, foo).Should().BeFalse();
            identifier.IsIdempotent(PATCH, foobar).Should().BeFalse();
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