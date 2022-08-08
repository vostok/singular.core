using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal class IclCache : ISettingsCache<IdempotencyControlRule>
    {
        private static readonly IdempotencyControlRule DefaultIdempotencyRule = new IdempotencyControlRule
        {
            Method = "*",
            PathPattern = new Wildcard("*"),
            IsIdempotent = true,
            OverrideHeader = false
        };
        private readonly CachingTransformAsync<IdempotencySettings, List<IdempotencyControlRule>> cache;

        public IclCache(IIclRulesSettingsProvider iclRulesSettingsProvider)
        {
            cache = new CachingTransformAsync<IdempotencySettings, List<IdempotencyControlRule>>(PreprocessSettings, iclRulesSettingsProvider.GetAsync);
        }

        public async Task<List<IdempotencyControlRule>> GetAsync()
        {
            return await cache.GetAsync().ConfigureAwait(false);
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
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern.TrimStart('/')),
                        OverrideHeader = r.OverrideHeader
                    })
                .Append(DefaultIdempotencyRule)
                .ToList();
        }
    }
}