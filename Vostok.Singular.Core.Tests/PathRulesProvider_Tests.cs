using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns;
using Vostok.Singular.Core.PathPatterns.PathRules;

namespace Vostok.Singular.Core.Tests
{
    public class PathRulesProvider_Tests
    {
        private PathRulesProvider pathRulesProvider;

        [SetUp]
        public void SetUp()
        {
            var settingsProvider = Substitute.For<ISettingsProvider>();
            settingsProvider.GetAsync(Arg.Any<SingularSettings>())
                .Returns(new SingularSettings
                {
                    PathPatternSigns = new SingularSettings.PathPatternSettings
                    {
                        Rules = new List<PathSettingsRule>
                        {
                            new() {Method = "*", PathPattern = "*/get-smth*", SettingsAlias = "get-smth-alias"},
                            new() {Method = "POST", PathPattern = "*", SettingsAlias = "testAlias"},
                            new() {Method = "GET", PathPattern = "/testWithSlash", SettingsAlias = "testAliasWithSlash"},
                            new() {Method = "GET", PathPattern = "testWithoutSlash", SettingsAlias = "testAliasWithoutSlash"}
                        }
                    }
                });
            pathRulesProvider = new PathRulesProvider(new PathRulesCache(new SingularServiceSettingsProvider(settingsProvider)));
        }

        [Test]
        public async Task Should_return_null_when_no_aliases()
        {
            var result = await pathRulesProvider.Get("*", "*");

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
            var result = await pathRulesProvider.Get(method, path);

            result.SettingsAlias.Should().Be(alias);
        }

        [TestCase("GET", "/test/haha")]
        [TestCase("PUT", "*")]
        public async Task Should_return_null_when_no_matches(string method, string path)
        {
            var result = await pathRulesProvider.Get(method, path);

            result.Should().BeNull();
        }
    }
}