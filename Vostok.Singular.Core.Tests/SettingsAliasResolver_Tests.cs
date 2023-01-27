using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.Tests
{
    public class SettingsAliasResolver_Tests
    {
        private SettingsAliasResolver settingsAliasIdentifier;

        [SetUp]
        public void SetUp()
        {
            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.GetAsync(Arg.Any<SingularSettings.PathPatternSettings>())
                .Returns(new SingularSettings.PathPatternSettings
                {
                    Rules = new List<PathSettingsRule>
                    {
                        new() {Method = "*", PathPattern = "*/get-smth*", SettingsAlias = "get-smth-alias"},
                        new() {Method = "POST", PathPattern = "*", SettingsAlias = "testAlias"},
                        new() {Method = "GET", PathPattern = "/testWithSlash", SettingsAlias = "testAliasWithSlash"},
                        new() {Method = "GET", PathPattern = "testWithoutSlash", SettingsAlias = "testAliasWithoutSlash"}
                    }
                });
            settingsAliasIdentifier = new SettingsAliasResolver(new PathPatternCache(settingsProvider));
        }

        [Test]
        public async Task Should_return_null_when_no_aliases()
        {
            var result = await settingsAliasIdentifier.GetPathPatternRuleAsync("*", "*");

            result.Should().BeNull();
        }

        [TestCase("POST", "/test/haha", "testAlias")]
        [TestCase("GET", "sasd/get-smth-value", "get-smth-alias")]
        [TestCase("POST", "abra/get-smth", "get-smth-alias")]
        [TestCase("GET", "testWithSlash", "testAliasWithSlash")]
        [TestCase("GET", "/testWithSlash", "testAliasWithSlash")]
        [TestCase("GET", "tEsTWItHSLaSh", "testAliasWithSlash")]
        [TestCase("GET", "/testWithSlash", "testAliasWithSlash")]
        [TestCase("GET", "testWithoutSlash", "testAliasWithoutSlash")]
        [TestCase("GET", "/TeStwiThOuTSLaSh", "testAliasWithoutSlash")]
        public async Task Should_return_alias_when_path_matched(string method, string path, string alias)
        {
            var result = await settingsAliasIdentifier.GetPathPatternRuleAsync(method, path);

            result.SettingsAlias.Should().Be(alias);
        }

        [TestCase("GET", "/test/haha")]
        [TestCase("PUT", "*")]
        public async Task Should_return_null_when_no_matches(string method, string path)
        {
            var result = await settingsAliasIdentifier.GetPathPatternRuleAsync(method, path);

            result.Should().BeNull();
        }
    }
}