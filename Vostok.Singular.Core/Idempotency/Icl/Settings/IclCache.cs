using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    internal class IclCache : IIclCache
    {
        private readonly CachingTransform<IclRulesServiceSettings, IdempotencyControlRule[]> cache;

        public IclCache(SettingsProvider iclSettingsProvider)
        {
            cache = new CachingTransform<IclRulesServiceSettings, IdempotencyControlRule[]>(
                PreprocessSigns,
                () => iclSettingsProvider.Get(new IclRulesServiceSettings
                {
                    Settings = new IclRulesSettings
                    {
                        Rules = new List<IdempotencyRuleSetting>(0)
                    }
                }));
        }

        public IdempotencyControlRule[] Get()
        {
            return cache.Get();
        }

        private static IdempotencyControlRule[] PreprocessSigns(IclRulesServiceSettings idempotencyControlSettings)
        {
            return idempotencyControlSettings
                .Settings
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