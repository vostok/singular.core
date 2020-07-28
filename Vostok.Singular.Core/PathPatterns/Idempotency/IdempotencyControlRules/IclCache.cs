using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal class IclCache : ISettingsCache<IdempotencyControlRule>
    {
        private static readonly IdempotencyControlRule DefaultIdempotencyRule = new IdempotencyControlRule
        {
            Method = "*",
            PathPattern = new Wildcard("*"),
            IsIdempotent = true
        };
        private readonly CachingTransform<IdempotencySettings, List<IdempotencyControlRule>> cache;

        public IclCache(IIclRulesSettingsProvider iclRulesSettingsProvider)
        {
            cache = new CachingTransform<IdempotencySettings, List<IdempotencyControlRule>>(PreprocessSettings, iclRulesSettingsProvider.Get);
        }

        public List<IdempotencyControlRule> Get()
        {
            return cache.Get();
        }

        private static List<IdempotencyControlRule> PreprocessSettings(IdempotencySettings idempotencySettings)
        {
            return idempotencySettings
                .Rules
                .Select(
                    r => new IdempotencyControlRule
                    {
                        Method = r.Method,
                        IsIdempotent = r.IsIdempotent,
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern.TrimStart('/'))
                    })
                .Append(DefaultIdempotencyRule)
                .ToList();
        }
    }
}