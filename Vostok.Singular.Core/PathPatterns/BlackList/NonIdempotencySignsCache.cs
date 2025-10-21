using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal class NonIdempotencySignsCache : ISettingsCache<NonIdempotencySign>
    {
        private readonly ValueTaskCachingTransformAsync<NonIdempotencySignsSettings, List<NonIdempotencySign>> cache;

        public NonIdempotencySignsCache(INonIdempotencySignsSettingsProvider nonIdempotencySignsSettingsProvider)
        {
            cache = new ValueTaskCachingTransformAsync<NonIdempotencySignsSettings, List<NonIdempotencySign>>(
                PreprocessSigns,
                nonIdempotencySignsSettingsProvider.GetAsync);
        }

        public async ValueTask<List<NonIdempotencySign>> GetAsync()
        {
            return await cache.GetAsync().ConfigureAwait(false);
        }

        private static List<NonIdempotencySign> PreprocessSigns(NonIdempotencySignsSettings nonIdempotencySignsSettings)
        {
            var signs = nonIdempotencySignsSettings.Signs;
            var processedSigns = new List<NonIdempotencySign>(signs.Count);

            processedSigns.AddRange(
                signs.Select(
                    sign => new NonIdempotencySign(
                        sign.Method,
                        sign.PathPattern
                    )
                ));

            return processedSigns;
        }
    }
}