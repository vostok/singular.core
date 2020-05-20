using Vostok.Singular.Core.Idempotency.Icl.Settings;

namespace Vostok.Singular.Core.Idempotency.Icl
{
    internal class IdempotencyControlRule
    {
        public string Method;

        public Wildcard PathPattern;

        public IdempotencyRuleType Type;
    }
}