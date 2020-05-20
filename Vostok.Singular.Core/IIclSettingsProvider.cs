using System.Collections.Generic;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core
{
    internal interface IIclSettingsProvider
    {
        List<IdempotencyRuleSetting> Get();
    }
}