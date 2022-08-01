using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal class IclResolver : IIclResolver
    {
        private readonly ISettingsCache<IdempotencyControlRule> iclCache;

        public IclResolver(ISettingsCache<IdempotencyControlRule> iclCache)
        {
            this.iclCache = iclCache;
        }

        public async Task<bool> IsIdempotentAsync(string method, string path)
        {
            var rules = await iclCache.GetAsync().ConfigureAwait(false);
            //We are assume here that last rule is always {* * Idempotent}. See IclCache.
            if (rules.Count > 1 && path.StartsWith("/"))
                path = path.TrimStart('/');

            var matchedRule = rules.First(r => PathPatternRuleMatcher.IsMatch(r, method, path));

            return matchedRule.IsIdempotent;
        }
    }
}