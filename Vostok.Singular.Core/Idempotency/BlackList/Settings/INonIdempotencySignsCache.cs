using System.Collections.Generic;

namespace Vostok.Singular.Core.Idempotency.BlackList.Settings
{
    internal interface INonIdempotencySignsCache
    {
        List<NonIdempotencySign> Get();
    }
}