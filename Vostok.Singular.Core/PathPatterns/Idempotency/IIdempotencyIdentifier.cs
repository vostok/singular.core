namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        bool IsIdempotent(string method, string path);
    }
}