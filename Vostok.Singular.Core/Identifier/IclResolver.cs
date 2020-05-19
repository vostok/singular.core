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
        private readonly IdempotencyControlRule defualtIdempotencyRule = new IdempotencyControlRule
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
            var rules = iclCache.Get().Append(defualtIdempotencyRule);
            var matchedRule = rules.First(r => IsMatch(r, method, path));

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

        private static bool IsMethodMatched(IdempotencyControlRule rule, string method) =>
            string.Equals(rule.Method, method, StringComparison.OrdinalIgnoreCase)
            || rule.Method == "*";

        private bool IsMatch(IdempotencyControlRule rule, string method, string path)
        {
            if (rule.Method == null || rule.PathPattern == null)
                return false;

            if (!IsMethodMatched(rule, method) || path == null)
                return false;

            if (rule.PathPattern.IsMatch(path))
            {
                return true;
            }

            return false;
        }
    }
}