using System.Linq;

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
            IsIdempotent = true
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

            return matchedRule.IsIdempotent;
        }
    }
}