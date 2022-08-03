using System.Linq;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly IBlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IIclResolver iclResolver;
        private readonly ISettingsCache<IdempotencyControlRule> idempotencySettingsCache;

        public IdempotencyIdentifier(
            IBlackListIdempotencyResolver blackListIdempotencyResolver,
            IIclResolver iclResolver,
            ISettingsCache<IdempotencyControlRule> idempotencySettingsCache)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
            this.idempotencySettingsCache = idempotencySettingsCache;
        }

        public async Task<bool> IsIdempotentAsync(string method, string path, string headerValue)
        {
            var rules = await idempotencySettingsCache.GetAsync().ConfigureAwait(false);

            if (rules.Count > 1 && path.StartsWith("/"))
                path = path.TrimStart('/');

            var matchedRule = rules.First(r => PathPatternRuleMatcher.IsMatch(r, method, path));

            if (matchedRule.OverrideHeader && bool.TryParse(headerValue, out var value))
                return value;
            
            return await blackListIdempotencyResolver.IsIdempotent(method, path).ConfigureAwait(false) && await iclResolver.IsIdempotentAsync(method, path).ConfigureAwait(false);
        }
    }
}