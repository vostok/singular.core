using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly IBlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IIclResolver iclResolver;

        public IdempotencyIdentifier(
            IBlackListIdempotencyResolver blackListIdempotencyResolver,
            IIclResolver iclResolver)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
        }

        public async Task<bool> IsIdempotentAsync(string method, string path, string headerValue)
        {
            var idempotentByHeader = HeaderIdempotencyResolver.IsIdempotentByHeader(headerValue);
            if (idempotentByHeader.HasValue)
                return idempotentByHeader.Value;

            return await blackListIdempotencyResolver.IsIdempotent(method, path).ConfigureAwait(false) && await iclResolver.IsIdempotentAsync(method, path).ConfigureAwait(false);
        }
    }
}