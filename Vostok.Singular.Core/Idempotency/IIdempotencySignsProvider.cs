using Vostok.Singular.Core.Idempotency.Settings;

namespace Vostok.Singular.Core.Idempotency
{
    internal interface IIdempotencySignsProvider
    {
        NonIdempotencySignsSettings Get();
    }
}