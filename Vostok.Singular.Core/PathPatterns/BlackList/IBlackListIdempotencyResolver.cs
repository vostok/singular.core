using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface IBlackListIdempotencyResolver
    {
        Task<bool> IsIdempotent(string method, string path);
    }
}