using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal class IclCache : ISettingsCache<IdempotencyControlRule>
    {
        private readonly CachingTransform<IclRulesServiceSettings, List<IdempotencyControlRule>> cache;

        public IclCache(SettingsProvider iclSettingsProvider)
        {
            cache = new CachingTransform<IclRulesServiceSettings, List<IdempotencyControlRule>>(
                PreprocessSigns,
                () => iclSettingsProvider.Get(new IclRulesServiceSettings
                {
                    Settings = new IclRulesSettings
                    {
                        Rules = new List<IdempotencyRuleSetting>(0)
                    }
                }));
        }

        public List<IdempotencyControlRule> Get()
        {
            return cache.Get();
        }

        private static List<IdempotencyControlRule> PreprocessSigns(IclRulesServiceSettings idempotencyControlSettings)
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
                .ToList();
        }
    }
}