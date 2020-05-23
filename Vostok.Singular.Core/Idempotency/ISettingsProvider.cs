namespace Vostok.Singular.Core.Idempotency
{
    internal interface ISettingsProvider
    {
        T Get<T>(T defaultValue);
    }
}