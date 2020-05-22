using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    internal class IclCache : IIclCache
    {
        private readonly CachingTransform<IclRulesSettings, IdempotencyControlRule[]> cache;

        public IclCache(IIclSettingsProvider iclSettingsProvider)
        {
            cache = new CachingTransform<IclRulesSettings, IdempotencyControlRule[]>(
                PreprocessSigns,
                iclSettingsProvider.Get);
        }

        public IdempotencyControlRule[] Get()
        {
            return cache.Get();
        }

        private static IdempotencyControlRule[] PreprocessSigns(IclRulesSettings idempotencyControlSettings)
        {
            return idempotencyControlSettings
                .Rules
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