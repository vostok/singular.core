using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal interface IIclRulesSettingsProvider
    {
        //CR: (deniaa) � C# ������� �������� ������, ������������ Task, � ��������� Async!
        Task<IdempotencySettings> Get();
    }
}