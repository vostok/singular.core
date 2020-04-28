using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core
{
    internal interface IIdempotencySignsProvider
    {
        NonIdempotencySignsSettings Get();
    }
}