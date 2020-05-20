namespace Vostok.Singular.Core.Idempotency.BlackList.Settings
{
    internal interface IIdempotencySignsProvider
    {
        NonIdempotencySignsSettings Get();
    }
}