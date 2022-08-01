using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal interface IHeaderIdempotencyResolver2
    {
        Task<bool?> IsIdempotentAsync(string header);
    }
}