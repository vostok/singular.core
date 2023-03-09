using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsProvider
    {
        Task<T> GetAsync<T>(T defaultValue);
    }
}