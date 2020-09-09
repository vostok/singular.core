using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface INonIdempotencySignsSettingsProvider
    {
        //CR: (deniaa) В C# принято называть методы, возвращающие Task, с суффиксом Async!
        Task<NonIdempotencySignsSettings> Get();
    }
}