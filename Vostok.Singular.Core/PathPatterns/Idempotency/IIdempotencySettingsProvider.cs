namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencySettingsProvider<T>
    {
        T Get(T defaultValue);
    }
}