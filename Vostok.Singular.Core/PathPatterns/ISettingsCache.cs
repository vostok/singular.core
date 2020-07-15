using System.Collections.Generic;

namespace Vostok.Singular.Core.Idempotency
{
    internal interface ISettingsCache<TSettings>
    {
        List<TSettings> Get();
    }
}