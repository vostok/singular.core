using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.Idempotency.BlackList.Settings
{
    // CR: А тут имя файлика с именем класса разошлось. И если еще что-то переименовывал, то тоже проверь.
    // Зачем этим кэшам разные интерфейсы? Одного достаточно
    internal class NonIdempotencySignsCache : INonIdempotencySignsCache
    {
        private CachingTransform<NonIdempotencySignsSettings, List<NonIdempotencySign>> cache;

        public NonIdempotencySignsCache(IIdempotencySignsProvider idempotencySignsProvider)
        {
            cache = new CachingTransform<NonIdempotencySignsSettings, List<NonIdempotencySign>>(PreprocessSigns,
                idempotencySignsProvider.Get);
        }

        public List<NonIdempotencySign> Get()
        {
            return cache.Get();
        }

        private static List<NonIdempotencySign> PreprocessSigns(NonIdempotencySignsSettings nonIdempotencySignsSettings)
        {
            var signs = nonIdempotencySignsSettings.Signs;
            var processedSigns = new List<NonIdempotencySign>(signs.Count);

            processedSigns.AddRange(
                signs.Select(
                    sign => new NonIdempotencySign
                    {
                        Method = sign.Method,
                        PathPattern = sign.PathPattern == null ? null : new Wildcard(sign.PathPattern)
                    }));

            return processedSigns;
        }
    }
}