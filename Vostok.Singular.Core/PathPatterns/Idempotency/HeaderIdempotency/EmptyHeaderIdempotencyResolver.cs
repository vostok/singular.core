using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal class EmptyHeaderIdempotencyResolver : IHeaderIdempotencyResolver
    {
        public Task<bool?> IsIdempotentAsync(string method, string path, string header) => Task.FromResult<bool?>(null);
    }
}