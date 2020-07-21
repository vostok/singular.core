using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    public class NonIdempotencySingsCache_Tests
    {
        private INonIdempotencySignsSettingsProvider singsProvider;
        private NonIdempotencySignsCache cache;

        [SetUp]
        public void SetUp()
        {
            singsProvider = Substitute.For<INonIdempotencySignsSettingsProvider>();
            cache = new NonIdempotencySignsCache(singsProvider);
        }

        [Test]
        public void Should_trim_start_slash()
        {
            var settings = new NonIdempotencySignsSettings()
            {
                Signs = new List<NonIdempotencySignSettings>
                {
                    new NonIdempotencySignSettings()
                    {
                        Method = "Post",
                        PathPattern = "/test"
                    }
                }
            };
            singsProvider.Get().Returns(settings);

            var cached = cache.Get();
            cached.Count.Should().Be(1);

            cached[0].PathPattern.IsMatch("/test").Should().Be(false);

            cached[0].PathPattern.IsMatch("test").Should().Be(true);
        }

    }
}