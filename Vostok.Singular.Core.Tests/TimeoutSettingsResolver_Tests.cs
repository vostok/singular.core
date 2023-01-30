using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;
using Vostok.Singular.Core.PathPatterns.Timeout;

namespace Vostok.Singular.Core.Tests
{
    public class TimeoutSettingsResolver_Tests
    {
        private ISettingsProvider aliasSettingsProvider;
        private TimeoutSettingsResolver timeoutResolver;
        private ISettingsProvider commonSettingsProvider;

        [SetUp]
        public void SetUp()
        {
            aliasSettingsProvider = Substitute.For<ISettingsProvider>();
            commonSettingsProvider = Substitute.For<ISettingsProvider>();
            var aliasResolver = new SettingsAliasResolver(new PathPatternCache(aliasSettingsProvider));
            timeoutResolver = new TimeoutSettingsResolver(aliasResolver, commonSettingsProvider);
        }

        [Test]
        public async Task Should_return_path_pattern_timeout_if_default_timeout_not_set()
        {
            SetupPathPatternRule("*", "test", 10.Seconds());

            var result = await timeoutResolver.Get("GET", "test");

            result.Should().Be(10.Seconds());
        }

        [Test]
        public async Task Should_return_path_pattern_timeout_if_default_timeout_set()
        {
            SetupDefaultTimeout(30.Seconds());
            SetupPathPatternRule("*", "test", 10.Seconds());

            var result = await timeoutResolver.Get("GET", "test");

            result.Should().Be(10.Seconds());
        }

        [Test]
        public async Task Should_return_default_timeout_if_path_pattern_rule_not_found()
        {
            SetupDefaultTimeout(20.Seconds());

            var result = await timeoutResolver.Get("GET", "test");

            result.Should().Be(20.Seconds());
        }

        [Test]
        public async Task Should_return_default_timeout_if_path_pattern_timeout_not_set()
        {
            SetupDefaultTimeout(20.Seconds());
            SetupPathPatternRule("*", "test");

            var result = await timeoutResolver.Get("GET", "not-test");

            result.Should().Be(20.Seconds());
        }

        [Test]
        public async Task Should_return_default_timeout_if_path_pattern_rule_doesnt_match()
        {
            SetupDefaultTimeout(20.Seconds());
            SetupPathPatternRule("*", "test", 10.Seconds());

            var result = await timeoutResolver.Get("GET", "not-test");

            result.Should().Be(20.Seconds());
        }
        
        [Test]
        public async Task Should_return_null_if_no_overrides()
        {
            var result = await timeoutResolver.Get("GET", "test");

            result.Should().BeNull();
        }

        private void SetupDefaultTimeout(TimeSpan defaultTimeout)
        {
            commonSettingsProvider.GetAsync(Arg.Any<SingularSettings>())
                .Returns(new SingularSettings
                {
                    Defaults = new SingularSettings.DefaultsSettings
                    {
                        TimeBudget = defaultTimeout
                    }
                });
        }

        private void SetupPathPatternRule(string method, string path, TimeSpan? timeout = null)
        {
            aliasSettingsProvider.GetAsync(Arg.Any<SingularSettings>())
                .Returns(new SingularSettings
                {
                    PathPatternSigns = new SingularSettings.PathPatternSettings
                    {
                        Rules = new List<PathSettingsRule>
                        {
                            new PathSettingsRule {Method = method, PathPattern = path, TimeBudget = timeout},
                        }
                    }
                });
        }
    }
}