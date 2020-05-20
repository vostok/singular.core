namespace Vostok.Singular.Core.Idempotency.Icl.Settings
{
    internal interface IIclCache
    {
        IdempotencyControlRule[] Get();
    }
}