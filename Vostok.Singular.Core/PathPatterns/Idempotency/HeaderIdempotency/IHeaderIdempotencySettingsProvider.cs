using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal interface IHeaderIdempotencySettingsProvider
    {
        Task<IdempotencyHeaderSettings> GetAsync();
    }
}