using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Tests
{
    public class IclCache_Tests
    {
        private IIclRulesSettingsProvider singsProvider;
        private IclCache cache;

        [SetUp]
        public void SetUp()
        {
            singsProvider = Substitute.For<IIclRulesSettingsProvider>();
            cache = new IclCache(singsProvider);
        }

        [Test]
        public void Should_always_add_the_last_rule()
        {
            var settings = new IdempotencySettings
            {
                Rules = new List<IdempotencyRuleSetting>
                {
                    new IdempotencyRuleSetting
                    {
                        Method = "Post",
                        PathPattern = "/test",
                        IsIdempotent = false,
                        OverrideHeader = false
                    }
                }
            };

            singsProvider.GetAsync().Returns(settings);

            var cached = cache.GetAsync().GetAwaiter().GetResult();
            cached.Count.Should().Be(2);

            cached.Last().Method.Should().Be("*");
            cached.Last().IsIdempotent.Should().Be(true);
        }

        [Test]
        public void Should_trim_start_slash()
        {
            var settings = new IdempotencySettings
            {
                Rules = new List<IdempotencyRuleSetting>
                {
                    new IdempotencyRuleSetting
                    {
                        Method = "Post",
                        PathPattern = "/test",
                        IsIdempotent = false,
                        OverrideHeader = false
                    }
                }
            };
            singsProvider.GetAsync().Returns(settings);

            var cached = cache.GetAsync().GetAwaiter().GetResult();
            cached[0].PathPattern.IsMatch("/test").Should().Be(false);
            cached[0].PathPattern.IsMatch("test").Should().Be(true);
        }
    }
}