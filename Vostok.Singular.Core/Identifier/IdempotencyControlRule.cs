using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core.Identifier
{
    internal class IdempotencyControlRule
    {
        public string Method;

        public Wildcard PathPattern;

        public IdempotencyRuleType Type;
    }
}