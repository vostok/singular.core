using System;

namespace Vostok.Singular.Core.Settings
{
    [Serializable]
    internal class IdempotencyRuleSetting
    {
        public string Method;

        public string PathPattern;

        public IdempotencyRuleType Type;
    }
}