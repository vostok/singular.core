using System;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings
{
    [Serializable]
    internal class IdempotencyRuleSetting
    {
        public string Method;

        public string PathPattern;

        public IdempotencyRuleType Type;
    }
}