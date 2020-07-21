using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings
{
    [Serializable]
    internal class IdempotencySettings
    {
        public List<IdempotencyRuleSetting> Rules = new List<IdempotencyRuleSetting>();
    }
}