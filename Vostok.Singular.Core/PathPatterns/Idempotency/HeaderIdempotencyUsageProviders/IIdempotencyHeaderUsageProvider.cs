using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal interface IIdempotencyHeaderUsageProvider
    {
        Task<bool> CanUseHeader(string method, string path);
    }
}