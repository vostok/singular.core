namespace Vostok.Singular.Core.Identifier
{
    internal interface IIdempotencyIdentifier
    {
        bool IsIdempotent(string method, string path);
    }
}