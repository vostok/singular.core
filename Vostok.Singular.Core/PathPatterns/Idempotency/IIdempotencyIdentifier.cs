using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        ValueTask<bool> IsIdempotentAsync(string method, string path, string headerValue = null);
    }
}