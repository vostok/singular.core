using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsCache<TSettings>
    {
        Task<List<TSettings>> GetAsync();
    }
}