using System.Collections.Generic;
using System.Linq;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal class IclCache : SettingsCacheBase<IdempotencySettings, IdempotencyControlRule>
    {
        private static readonly IdempotencyControlRule DefaultIdempotencyRule = new IdempotencyControlRule
        {
            Method = "*",
            PathPattern = new Wildcard("*"),
            IsIdempotent = true
        };

        public IclCache(IIclRulesSettingsProvider iclRulesSettingsProvider)
            : base(iclRulesSettingsProvider.Get)
        {
        }

        protected override List<IdempotencyControlRule> PreprocessSettings(IdempotencySettings idempotencySettings)
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