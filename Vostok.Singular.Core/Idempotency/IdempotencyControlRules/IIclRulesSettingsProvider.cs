using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal interface IIclRulesSettingsProvider
    {
        IclRulesSettings Get();
    }
}