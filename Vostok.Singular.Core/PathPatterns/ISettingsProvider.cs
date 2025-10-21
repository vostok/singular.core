using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsProvider
    {
        ValueTask<T> GetAsync<T>(T defaultValue);
    }
}