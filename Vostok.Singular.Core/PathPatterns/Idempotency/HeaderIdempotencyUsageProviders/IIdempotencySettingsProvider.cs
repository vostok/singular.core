using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal interface IIdempotencySettingsProvider<T>
    {
        Task<T> GetSettingsAsync();
    }
}