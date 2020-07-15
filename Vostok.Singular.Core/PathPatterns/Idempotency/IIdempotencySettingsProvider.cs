namespace Vostok.Singular.Core.Idempotency
{
    internal interface IIdempotencySettingsProvider<T>
    {
        T Get(T defaultValue);
    }
}