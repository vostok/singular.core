using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Singular.Core.Idempotency.Identifier;
using Vostok.Singular.Core.Identifier;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core
{
    internal class IclCache : IIclCache
    {
        private readonly CachingTransform<List<IdempotencyRuleSetting>, IdempotencyControlRule[]> cache;

        public IclCache(IIclSettingsProvider iclSettingsProvider)
        {
            cache = new CachingTransform<List<IdempotencyRuleSetting>, IdempotencyControlRule[]>(
                PreprocessSigns,
                iclSettingsProvider.Get);
        }

        public IdempotencyControlRule[] Get()
        {
            return cache.Get();
        }

        private static IdempotencyControlRule[] PreprocessSigns(List<IdempotencyRuleSetting> idempotencyControlSettings)
        {
            return idempotencyControlSettings
                .Select(
                    r => new IdempotencyControlRule
                    {
                        Method = r.Method,
                        Type = r.Type,
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern)
                    })
                .ToArray();
        }
    }
}