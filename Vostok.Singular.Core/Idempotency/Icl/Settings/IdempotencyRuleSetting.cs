using System;

namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    [Serializable]
    internal class IdempotencyRuleSetting
    {
        public string Method;

        public string PathPattern;

        public IdempotencyRuleType Type;
    }
}