using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal interface IIdempotencyIdentifier
    {
        //CR: (deniaa) В C# принято называть методы, возвращающие Task, с суффиксом Async!
        Task<bool> IsIdempotent(string method, string path);
    }
}