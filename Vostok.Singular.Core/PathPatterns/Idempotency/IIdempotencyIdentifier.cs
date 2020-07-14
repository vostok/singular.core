namespace Vostok.Singular.Core.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        bool IsIdempotent(string method, string path);
    }
}