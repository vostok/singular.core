using System.Collections.Generic;

namespace Vostok.Singular.Core.Idempotency
{
    internal interface IIdempotencySettingsCache<TSettings>
    {
        List<TSettings> Get();
    }
}