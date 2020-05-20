using System;
using System.Linq;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core.Identifier
{
    /// <summary>
    /// ICL - Idempotency Control List, аналог ACL (Access Control List)
    /// </summary>
    internal class IclResolver
    {
        private readonly IIclCache iclCache;
        private readonly IdempotencyControlRule defaultIdempotencyRule = new IdempotencyControlRule
        {
            Method = "*",
            PathPattern = new Wildcard("*"),
            Type = IdempotencyRuleType.Idempotent
        };

        public IclResolver(
            IIclCache iclCache
        )
        {
            this.iclCache = iclCache;
        }

        public bool IsIdempotent(string method, string path)
        {
            var rules = iclCache.Get().Append(defaultIdempotencyRule);
            var matchedRule = rules.First(r => IclRuleMatcher.IsMatch(r, method, path));

            return IsIdempotent(matchedRule.Type);
        }

        private static bool IsIdempotent(IdempotencyRuleType ruleType)
        {
            switch (ruleType)
            {
                case IdempotencyRuleType.Idempotent:
                    return true;
                case IdempotencyRuleType.NonIdempotent:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}