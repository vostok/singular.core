using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal interface IIclRulesSettingsProvider
    {
        IdempotencySettings Get();
    }
}