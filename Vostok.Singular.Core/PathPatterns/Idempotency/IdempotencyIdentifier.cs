using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly BlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IclResolver iclResolver;

        public IdempotencyIdentifier(
            BlackListIdempotencyResolver blackListIdempotencyResolver,
            IclResolver iclResolver)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
        }

        public async Task<bool> IsIdempotent(string method, string path)
        {
            return await blackListIdempotencyResolver.IsIdempotent(method, path).ConfigureAwait(false) && await iclResolver.IsIdempotent(method, path).ConfigureAwait(false);
        }
    }
}