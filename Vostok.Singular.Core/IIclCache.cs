using Vostok.Singular.Core.Identifier;

namespace Vostok.Singular.Core
{
    internal interface IIclCache
    {
        IdempotencyControlRule[] Get();
    }
}