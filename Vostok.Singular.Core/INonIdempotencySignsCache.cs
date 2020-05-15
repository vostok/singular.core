using System.Collections.Generic;
using Vostok.Singular.Core.Idempotency.Identifier;

namespace Vostok.Singular.Core.Idempotency
{
    internal interface INonIdempotencySignsCache
    {
        List<NonIdempotencySign> Get();
    }
}