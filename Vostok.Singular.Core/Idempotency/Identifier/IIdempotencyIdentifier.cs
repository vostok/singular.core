namespace Vostok.Singular.Core.Idempotency.Identifier
{
    internal interface IIdempotencyIdentifier
    {
        bool IsIdempotent(string method, string path);
    }
}