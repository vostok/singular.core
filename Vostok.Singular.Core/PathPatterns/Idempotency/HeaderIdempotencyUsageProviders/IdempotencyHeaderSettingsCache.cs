using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class IdempotencyHeaderSettingsCache
    {
        private readonly CachingTransformAsync<IdempotencyHeaderSettings, bool> cache;

        public IdempotencyHeaderSettingsCache(IHeaderIdempotencySettingsProvider settingsProvider)
        {
            cache = new CachingTransformAsync<IdempotencyHeaderSettings, bool>(PreprocessSettings, settingsProvider.GetAsync);
        }

        public async Task<bool> GetAsync()
        {
            return await cache.GetAsync().ConfigureAwait(false);
        }

        private static bool PreprocessSettings(IdempotencyHeaderSettings settings)
        {
            return settings.OverrideHeader;
        }
    }
}