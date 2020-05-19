using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Identifier;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core.Tests
{
    public class IclResolverTests
    {
        private const string POST = "POST";
        private const string GET = "GET";
        private const string PATCH = "PATCH";
        private string empty = new Uri("/", UriKind.Relative).OriginalString;
        private string foo = new Uri("/foo", UriKind.Relative).OriginalString;
        private string foobar = new Uri("/foo/bar", UriKind.Relative).OriginalString;

        private IIclCache iclCache;
        private IclResolver iclResolver;

        [SetUp]
        public void SetUp()
        {
            iclCache = Substitute.For<IIclCache>();

            iclResolver = new IclResolver(iclCache);
        }

        [Test]
        public void Should_Be_Idempotent_When_NoRules()
        {
            iclResolver.IsIdempotent(POST, foo).Should().BeTrue();
        }

        [Test]
        public void Should_Be_NonIdempotent_When_NonIdempotentRule()
        {
            iclCache.Get()
                .Returns(
                    new[]
                    {
                        new IdempotencyControlRule
                        {
                            Method = "*",
                            Type = IdempotencyRuleType.NonIdempotent,
                            PathPattern = new Wildcard("*")
                        }
                    });

            iclResolver.IsIdempotent(POST, foo).Should().BeFalse();
        }
    }
}