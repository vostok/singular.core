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

        public async ValueTask<IdempotencyControlRule> GetRuleAsync(string method, string path)
        {
            var rules = await iclCache.GetAsync().ConfigureAwait(false);

            if (rules.Count > 1 && path.StartsWith("/"))
                path = path.TrimStart('/');

            return rules.First(r => PathPatternRuleMatcher.IsMatch(r, method, path));
        }
    }
}