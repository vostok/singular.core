using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    /// <summary>
    /// ICL - Idempotency Control List, analog for ACL (Access Control List)
    /// </summary>
    internal class IclResolver
    {
        private readonly ISettingsCache<IdempotencyControlRule> iclCache;

        public IclResolver(ISettingsCache<IdempotencyControlRule> iclCache)
        {
            this.iclCache = iclCache;
        }

        //CR: (deniaa) В C# принято называть методы, возвращающие Task, с суффиксом Async!
        public async Task<bool> IsIdempotent(string method, string path)
        {
            var rules = await iclCache.Get().ConfigureAwait(false);
            //We are assume here that last rule is always {* * Idempotent}. See IclCache.
            if (rules.Count > 1 && path.StartsWith("/"))
                path = path.TrimStart('/');

            var matchedRule = rules.First(r => PathPatternRuleMatcher.IsMatch(r, method, path));

            return matchedRule.IsIdempotent;
        }
    }
}