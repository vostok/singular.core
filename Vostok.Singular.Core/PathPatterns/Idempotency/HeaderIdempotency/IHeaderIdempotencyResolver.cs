using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal interface IHeaderIdempotencyResolver
    {
        Task<bool?> IsIdempotentAsync(string method, string path, string header);
    }
}