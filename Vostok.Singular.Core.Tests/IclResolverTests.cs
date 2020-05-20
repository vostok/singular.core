using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency.Identifier;
using Vostok.Singular.Core.Identifier;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core.Tests
{
    public class IclResolverTests
    {
        private const string POST = "POST";
        private const string fooPath = "/foo";

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
            iclResolver.IsIdempotent(POST, fooPath).Should().BeTrue();
        }

        [TestCase("*", "*")]
        [TestCase(POST, fooPath)]
        public void Should_Be_NonIdempotent_When_NonIdempotentRule(string methodPattern , string pathPattern)
        {
            iclCache.Get()
                .Returns(
                    new[]
                    {
                        new IdempotencyControlRule
                        {
                            Method = methodPattern,
                            Type = IdempotencyRuleType.NonIdempotent,
                            PathPattern = new Wildcard(pathPattern)
                        }
                    });

            iclResolver.IsIdempotent(POST, fooPath).Should().BeFalse();
        }

        [TestCase("*", "*")]
        [TestCase(POST, "/foo")]
        public void Should_Be_Idempotent_When_IdempotentRule_Is_First(string methodPattern , string pathPattern)
        {
            iclCache.Get()
                .Returns(
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
                    });

            iclResolver.IsIdempotent(POST, fooPath).Should().BeTrue();
        }
    }
}