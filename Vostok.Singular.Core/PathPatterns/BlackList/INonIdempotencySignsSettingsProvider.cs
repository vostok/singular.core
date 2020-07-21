using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface INonIdempotencySignsSettingsProvider
    {
        NonIdempotencySignsSettings Get();
    }
}