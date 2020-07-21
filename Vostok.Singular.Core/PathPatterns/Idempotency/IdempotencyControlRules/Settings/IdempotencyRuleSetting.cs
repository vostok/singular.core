using System;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings
{
    [Serializable]
    internal class IdempotencyRuleSetting
    {
        public string Method;

        public string PathPattern;

        public bool IsIdempotent;
    }
}