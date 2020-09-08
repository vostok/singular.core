using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly CachingTransformAsync<IdempotencySettings, List<IdempotencyControlRule>> cache;

        public IclCache(IIclRulesSettingsProvider iclRulesSettingsProvider)
        {
            cache = new CachingTransformAsync<IdempotencySettings, List<IdempotencyControlRule>>(PreprocessSettings, iclRulesSettingsProvider.Get);
        }

        public async Task<List<IdempotencyControlRule>> Get()
        {
            return await cache.Get().ConfigureAwait(false);
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