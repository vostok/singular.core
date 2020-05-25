using System.Linq;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    /// <summary>
    /// ICL - Idempotency Control List, analog for ACL (Access Control List)
    /// </summary>
    internal class IclResolver
    {
        private static readonly IdempotencyControlRule DefaultIdempotencyRule = new IdempotencyControlRule
        {
            Method = "*",
            PathPattern = new Wildcard("*"),
            Type = IdempotencyRuleType.Idempotent
        };

        private readonly IIdempotencySettingsCache<IdempotencyControlRule> iclCache;

        public IclResolver(IIdempotencySettingsCache<IdempotencyControlRule> iclCache)
        {
            this.iclCache = iclCache;
        }

        public bool IsIdempotent(string method, string path)
        {
            var rules = iclCache.Get().Append(DefaultIdempotencyRule);
            var matchedRule = rules.First(r => IclRuleMatcher.IsMatch(r, method, path));

            return matchedRule.Type == IdempotencyRuleType.Idempotent;
        }
    }
}