using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class IdempotencyHeaderUsageProvider : IIdempotencyHeaderUsageProvider
    {
        private readonly PathSettingsRulesResolver pathSettingsRulesResolver;
        private readonly IdempotencyHeaderSettingsCache idempotencyHeaderSettingsCache;

        public IdempotencyHeaderUsageProvider(PathSettingsRulesResolver pathSettingsRulesResolver, IdempotencyHeaderSettingsCache idempotencyHeaderSettingsCache)
        {
            this.pathSettingsRulesResolver = pathSettingsRulesResolver;
            this.idempotencyHeaderSettingsCache = idempotencyHeaderSettingsCache;
        }

        public async Task<bool> CanUseHeader(string method, string path)
        {
            var canUseFromPath = await pathSettingsRulesResolver.CanUseHeader(method, path);

            if (canUseFromPath)
                return true;

            return await idempotencyHeaderSettingsCache.GetAsync().ConfigureAwait(false);
        }
    }
}