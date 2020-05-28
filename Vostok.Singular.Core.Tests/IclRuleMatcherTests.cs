using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Singular.Core.Idempotency;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.Tests
{
    //CR: (deniaa) There is complex comment about feature.

    //CR: (deniaa) 1. Trailing and leading slashes are significant. At first, look at BlackListIdempotencyResolverTests. Old tests written with leading slashes in rules.
    //CR: (deniaa) Then, look at Idempotency_Tests in Singular backend solution. All rules in tests written with leading slashes in path. Is you remove any one slashes from rule - the rule will not work.
    //CR: (deniaa) This is because the Kestrel always gives urls with a slash in the beginning.
    //CR: (deniaa) Then, easy to check, that existing or not leading slash on CLIENT side totally depends on how user write code.
    //CR: (deniaa) If user write request with leading slash - path will be given to IdempotencyIdentifier with leading slash.
    //CR: (deniaa) If user write request WITHOUT leading slash - path will be given to IdempotencyIdentifier WITHOUT leading slash.
    //CR: (deniaa) So, rule in Configuration, which are used on backend side (Kestrel) AND client side, must return same result for "same" path. But it is not.
    //CR: (deniaa) Regardless of the presence and absence of a slash, the code should work the same way and the user should hardly think about it.
    //CR: (deniaa) Yes, this bug was allowed during the first implementation of idempotency on the client side. The code was simply copied from the backend, where the slash was guaranteed by Kestrel.

    //CR: (deniaa) 2. We definitely need test on case with request on corner of the site (path is "/" or empty string "", i don't know).
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