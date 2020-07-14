using Vostok.Singular.Core.Idempotency.BlackList.Settings;

namespace Vostok.Singular.Core.Idempotency.BlackList
{
    internal interface INonIdempotencySignsSettingsProvider
    {
        NonIdempotencySignsSettings Get();
    }
}