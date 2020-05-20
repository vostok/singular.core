using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency.Identifier;
using Vostok.Singular.Core.Identifier;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    internal class IclRuleMatcherTests
    {
        [TestCaseSource(nameof(GenerateTestCases))]
        public void IsMatch_Should_Math_When_TestCases(IdempotencyControlRule rule, string method, string path, bool expected)
        {
            IclRuleMatcher.IsMatch(rule, method, path).Should().Be(expected);
        }

        private static IEnumerable<TestCaseData> GenerateTestCases()
        {
            var nullMethod = new IdempotencyControlRule
            {
                Method = null,
                PathPattern = new Wildcard("foo")
            };
            yield return new TestCaseData(nullMethod, "POST", "foo", false)
                .SetName("null method");

            var nullPath = new IdempotencyControlRule
            {
                Method = "POST",
                PathPattern = null
            };
            yield return new TestCaseData(nullPath, "POST", "foo", false)
                .SetName("null path pattern");

            var correctRule = new IdempotencyControlRule
            {
                Method = "POST",
                PathPattern = new Wildcard("foo")
            };
            yield return new TestCaseData(correctRule, "POST", null, false)
                .SetName("null path pattern");

            var anyMethodAnyPath = new IdempotencyControlRule
            {
                Method = "*",
                PathPattern = new Wildcard("*")
            };

            yield return new TestCaseData(anyMethodAnyPath, "POST", "foo", true)
                .SetName("any method with any path, concrete method, concrete path");
            yield return new TestCaseData(anyMethodAnyPath, "POST", "*", true)
                .SetName("any method with any path, concrete method, any path");
            yield return new TestCaseData(anyMethodAnyPath, "*", "*", true)
                .SetName("any method with any path, any method, any path");

            var concreteMethodAnyPath = new IdempotencyControlRule
            {
                Method = "POST",
                PathPattern = new Wildcard("*")
            };
            yield return new TestCaseData(concreteMethodAnyPath, "POST", "foo", true)
                .SetName("POST method with any path, concrete method, concrete path");
            yield return new TestCaseData(concreteMethodAnyPath, "POST", "*", true)
                .SetName("POST method with any path, concrete method, any path");
            yield return new TestCaseData(concreteMethodAnyPath, "*", "*", false)
                .SetName("POST method with any path, any method, any path");

            var anyMethodConcretePath = new IdempotencyControlRule
            {
                Method = "*",
                PathPattern = new Wildcard("foo")
            };
            yield return new TestCaseData(anyMethodConcretePath, "POST", "foo", true)
                .SetName("any method with concrete path, concrete method, concrete path");
            yield return new TestCaseData(anyMethodConcretePath, "POST", "*", false)
                .SetName("any method with concrete path, concrete method, any path");
            yield return new TestCaseData(anyMethodConcretePath, "*", "*", false)
                .SetName("any method with concrete path, any method, any path");

            var concreteMethodConcretePath = new IdempotencyControlRule
            {
                Method = "POST",
                PathPattern = new Wildcard("foo")
            };
            yield return new TestCaseData(concreteMethodConcretePath, "POST", "foo", true)
                .SetName("concrete method with concrete path, concrete method, concrete path");
            yield return new TestCaseData(concreteMethodConcretePath, "POST", "*", false)
                .SetName("concrete method with concrete path, concrete method, any path");
            yield return new TestCaseData(concreteMethodConcretePath, "*", "*", false)
                .SetName("concrete method with concrete path, any method, any path");

            var concreteOtherMethodConcretePath = new IdempotencyControlRule
            {
                Method = "GET",
                PathPattern = new Wildcard("foo")
            };
            yield return new TestCaseData(concreteOtherMethodConcretePath, "POST", "foo", false)
                .SetName("other method with concrete path, concrete method, concrete path");
            yield return new TestCaseData(concreteOtherMethodConcretePath, "POST", "*", false)
                .SetName("other method with concrete path, concrete method, any path");
            yield return new TestCaseData(concreteOtherMethodConcretePath, "*", "*", false)
                .SetName("other method with concrete path, any method, any path");
        }
    }
}