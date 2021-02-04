using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Singular.Core.PathPatterns;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    public class PathPatternRuleMatcher_Tests
    {
        private List<PathPatternRule> rules;

        [OneTimeSetUp]
        public void Setup()
        {
            rules = new List<PathPatternRule>
            {
                new PathPatternRule
                {
                    Method = "*",
                    PathPattern = new Wildcard("*")
                }
            };
        }

        [TestCase(RequestMethods.Get, "test/\nasd")]
        [TestCase(RequestMethods.Get, "test/\n\n\r\n")]
        [TestCase("SomethingMethod", "\r\nabracadabra")]
        [TestCase("SomethingMethod", "test/\n/bul'")]
        public void Should_match_paths_with_special_symbols(string method, string path)
        {
            rules.Count(r => PathPatternRuleMatcher.IsMatch(r, method, path)).Should().BeGreaterThan(0);
        }
    }
}