using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.Idempotency.BlackList.Settings
{
    // CR: А тут имя файлика с именем класса разошлось. И если еще что-то переименовывал, то тоже проверь.
    // Зачем этим кэшам разные интерфейсы? Одного достаточно
    internal class NonIdempotencySignsCache : INonIdempotencySignsCache
    {
        private static readonly NonIdempotencyServiceSettings EmptySigns = new NonIdempotencyServiceSettings
        {
            NonIdempotencySigns = new NonIdempotencySignsSettings
            {
                Signs = new List<NonIdempotencySignSettings>(0)
            }
        };
        private readonly CachingTransform<NonIdempotencyServiceSettings, List<NonIdempotencySign>> cache;

        public NonIdempotencySignsCache(SettingsProvider settingsProvider)
        {
            cache = new CachingTransform<NonIdempotencyServiceSettings, List<NonIdempotencySign>>(
                PreprocessSigns,
                () => settingsProvider.Get(EmptySigns));
        }

        public List<NonIdempotencySign> Get()
        {
            return cache.Get();
        }

        private static List<NonIdempotencySign> PreprocessSigns(NonIdempotencyServiceSettings nonIdempotencySignsSettings)
        {
            var signs = nonIdempotencySignsSettings.NonIdempotencySigns.Signs;
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