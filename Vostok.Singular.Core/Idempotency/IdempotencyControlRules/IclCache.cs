using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal class IclCache : ISettingsCache<IdempotencyControlRule>
    {
        private readonly CachingTransform<IclRulesSettings, List<IdempotencyControlRule>> cache;

        public IclCache(IIclRulesSettingsProvider iclRulesSettingsProvider)
        {
            cache = new CachingTransform<IclRulesSettings, List<IdempotencyControlRule>>(
                PreprocessSigns,
                iclRulesSettingsProvider.Get);
        }

        public List<IdempotencyControlRule> Get()
        {
            return cache.Get();
        }

        private static List<IdempotencyControlRule> PreprocessSigns(IclRulesSettings iclRulesSettings)
        {
            return iclRulesSettings
                .Rules
                .Select(
                    r => new IdempotencyControlRule
                    {
                        Method = r.Method,
                        IsIdempotent = r.IsIdempotent,
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern)
                    })
                .ToList();
        }
    }
}