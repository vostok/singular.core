using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Singular.Core.Identifier;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core
{
    internal class IclCache : IIclCache
    {
        private readonly CachingTransform<IdempotencyControlListSetting, IdempotencyControlRule[]> cache;

        public IclCache(IIclSettingsProvider iclSettingsProvider)
        {
            cache = new CachingTransform<IdempotencyControlListSetting, IdempotencyControlRule[]>(
                PreprocessSigns,
                iclSettingsProvider.Get);
        }

        public IdempotencyControlRule[] Get()
        {
            return cache.Get();
        }

        private static IdempotencyControlRule[] PreprocessSigns(IdempotencyControlListSetting idempotencyControlListSetting)
        {
            return idempotencyControlListSetting
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