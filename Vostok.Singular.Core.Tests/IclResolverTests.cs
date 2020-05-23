using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency;
using Vostok.Singular.Core.Idempotency.Icl;
using Vostok.Singular.Core.Idempotency.Icl.Settings;

namespace Vostok.Singular.Core.Tests
{
    public class IclResolverTests
    {
        private const string POST = "POST";
        private const string fooPath = "/foo";

        private ISettingsCache<IdempotencyControlRule> iclCache;
        private IclResolver iclResolver;

        [SetUp]
        public void SetUp()
        {
            iclCache = Substitute.For<ISettingsCache<IdempotencyControlRule>>();

            iclResolver = new IclResolver(iclCache);
        }

        [Test]
        public void Should_Be_Idempotent_When_NoRules()
        {
            iclResolver.IsIdempotent(POST, fooPath).Should().BeTrue();
        }

        [TestCase("*", "*")]
        [TestCase(POST, fooPath)]
        public void Should_Be_NonIdempotent_When_NonIdempotentRule(string methodPattern, string pathPattern)
        {
            iclCache.Get()
                .Returns(
                    new List<IdempotencyControlRule>(
                        new[]
                        {
                            new IdempotencyControlRule
                            {
                                Method = methodPattern,
                                Type = IdempotencyRuleType.NonIdempotent,
                                PathPattern = new Wildcard(pathPattern)
                            }
                        }));

            iclResolver.IsIdempotent(POST, fooPath).Should().BeFalse();
        }

        [TestCase("*", "*")]
        [TestCase(POST, "/foo")]
        public void Should_Be_Idempotent_When_IdempotentRule_Is_First(string methodPattern, string pathPattern)
        {
            iclCache.Get()
                .Returns(
                    new List<IdempotencyControlRule>(
                    new[]
                    {
                        new IdempotencyControlRule
                        {
                            Method = methodPattern,
                            Type = IdempotencyRuleType.Idempotent,
                            PathPattern = new Wildcard(pathPattern)
                        },
                        new IdempotencyControlRule
                        {
                            Method = methodPattern,
                            Type = IdempotencyRuleType.NonIdempotent,
                            PathPattern = new Wildcard(pathPattern)
                        },
                    }));

            iclResolver.IsIdempotent(POST, fooPath).Should().BeTrue();
        }
    }
}