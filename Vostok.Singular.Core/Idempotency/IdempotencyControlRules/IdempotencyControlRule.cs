using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{

    // CR: переименуй всю папку в IdempotencyControlRules (и неймспейсы в классах потом не забудь поправить)
    internal class IdempotencyControlRule
    {
        public string Method;

        public Wildcard PathPattern;

        public IdempotencyRuleType Type;
    }
}