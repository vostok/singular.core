using System.Linq;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    /// <summary>
    /// ICL - Idempotency Control List, analog for ACL (Access Control List)
    /// </summary>
    internal class IclResolver
    {
        private readonly IIdempotencySettingsCache<IdempotencyControlRule> iclCache;

        public IclResolver(IIdempotencySettingsCache<IdempotencyControlRule> iclCache)
        {
            this.iclCache = iclCache;
        }

        public bool IsIdempotent(string method, string path)
        {
            var rules = iclCache.Get();
            //We are assume here that last rule is always {* * Idempotent}. See IclCache.
            if (rules.Count > 1 && path.StartsWith("/"))
                path = path.TrimStart('/');

            var matchedRule = rules.First(r => IclRuleMatcher.IsMatch(r, method, path));

            return matchedRule.IsIdempotent;
        }
    }
}