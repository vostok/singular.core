using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class AliasRulesSettingsCache
    {
        private readonly CachingTransformAsync<Dictionary<string, SingularSettings>, Dictionary<string, SingularSettings>> cache;

        public AliasRulesSettingsCache(IIdempotencySettingsProvider<Dictionary<string, SingularSettings>> settingsProvider)
        {
            cache = new CachingTransformAsync<Dictionary<string, SingularSettings>,
                Dictionary<string, SingularSettings>>(x => x, settingsProvider.GetSettingsAsync);
        }

        public async Task<Dictionary<string, SingularSettings>> GetAsync()
        {
            return await cache.GetAsync();
        }
    }
}