using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class AliasRulesResolver
    {
        private readonly AliasRulesSettingsCache aliasRulesSettingsCache;

        public AliasRulesResolver(AliasRulesSettingsCache aliasRulesSettingsCache)
        {
            this.aliasRulesSettingsCache = aliasRulesSettingsCache;
        }

        public async Task<bool> CanUseHeader(string alias)
        {
            var aliasSettings = await aliasRulesSettingsCache.GetAsync();

            if (aliasSettings.TryGetValue(alias, out var settings))
                return settings?.IdempotencyHeaderSettings?.OverrideHeader ?? false;

            return false;
        }
    }
}