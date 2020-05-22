using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    [Serializable]
    internal class IclRulesSettings
    {
        public List<IdempotencyRuleSetting> Rules = new List<IdempotencyRuleSetting>();
    }
}