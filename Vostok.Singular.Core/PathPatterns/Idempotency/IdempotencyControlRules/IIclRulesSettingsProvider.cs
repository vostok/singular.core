using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal interface IIclRulesSettingsProvider
    {
        //CR: (deniaa) В C# принято называть методы, возвращающие Task, с суффиксом Async!
        Task<IdempotencySettings> Get();
    }
}