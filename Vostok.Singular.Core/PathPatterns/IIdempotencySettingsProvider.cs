using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface IIdempotencySettingsProvider<T>
    {
        Task<T> GetAsync();
    }
}