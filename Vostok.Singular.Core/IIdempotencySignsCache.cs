using System.Collections.Generic;
using Vostok.Singular.Core.Identifier;

namespace Vostok.Singular.Core
{
    internal interface IIdempotencySignsCache
    {
        List<NonIdempotencySign> Get();
    }
}