using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        Task<bool> IsIdempotentAsync(string method, string path);
    }
}