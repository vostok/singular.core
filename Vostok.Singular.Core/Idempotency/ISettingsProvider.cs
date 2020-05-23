namespace Vostok.Singular.Core.Idempotency
{
    internal interface ISettingsProvider<T>
    {
        T Get(T defaultValue);
    }
}