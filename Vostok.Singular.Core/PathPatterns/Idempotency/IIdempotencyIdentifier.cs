using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        Task<bool> IsIdempotent(string method, string path);
    }
}