using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    /// <summary>
    /// ICL - Idempotency Control List, analog for ACL (Access Control List)
    /// </summary>
    internal interface IIclResolver
    {
        Task<bool> IsIdempotentAsync(string method, string path);
    }
}