using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal interface IBlackListIdempotencyResolver
    {
        ValueTask<bool> IsIdempotent(string method, string path);
    }
}