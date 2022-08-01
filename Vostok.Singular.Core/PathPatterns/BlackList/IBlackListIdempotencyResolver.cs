using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface IBlackListIdempotencyResolver
    {
        Task<bool> IsIdempotentAsync(string method, string path);
    }
}