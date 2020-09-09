using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsCache<TSettings>
    {
        //CR: (deniaa) В C# принято называть методы, возвращающие Task, с суффиксом Async!
        Task<List<TSettings>> Get();
    }
}