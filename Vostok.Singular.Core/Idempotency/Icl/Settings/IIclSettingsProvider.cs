using System.Collections.Generic;

namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    internal interface IIclSettingsProvider
    {
        List<IdempotencyRuleSetting> Get();
    }
}