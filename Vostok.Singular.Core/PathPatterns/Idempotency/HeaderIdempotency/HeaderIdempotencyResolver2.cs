using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal class HeaderIdempotencyResolver2 : IHeaderIdempotencyResolver2
    {
        private readonly CachingTransformAsync<IdempotencyHeaderSettings, bool> cache;

        public HeaderIdempotencyResolver2(IIdempotencySettingsProvider<IdempotencyHeaderSettings> settingsProvider)
        {
            cache = new CachingTransformAsync<IdempotencyHeaderSettings, bool>
                (PreprocessSettings, settingsProvider.GetAsync);
        }

        public async Task<bool?> IsIdempotentAsync(string headerValue)
        {
            var idempotenceByHeaderOn = await cache.GetAsync();
            
            if (!idempotenceByHeaderOn)
                return null;
            
            if (!bool.TryParse(headerValue, out var isIdempotent))
                return null;
            
            return isIdempotent;
        }
        
        private static bool PreprocessSettings(IdempotencyHeaderSettings settings)
        {
            return settings.OverrideHeader;
        }
    }
}