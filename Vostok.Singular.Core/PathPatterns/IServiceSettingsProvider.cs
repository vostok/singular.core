using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface IServiceSettingsProvider
    {
        Task<SingularSettings> Get();
    }
}