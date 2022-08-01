using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly IBlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IIclResolver iclResolver;
        private readonly IHeaderIdempotencyResolver2 headerIdempotencyResolver;

        public IdempotencyIdentifier(
            IBlackListIdempotencyResolver blackListIdempotencyResolver,
            IIclResolver iclResolver,
            IHeaderIdempotencyResolver2 headerIdempotencyResolver)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
            this.headerIdempotencyResolver = headerIdempotencyResolver;
        }

        public async Task<bool> IsIdempotentAsync(string method, string path, string headerValue)
        {
            var idempotentByHeader = await headerIdempotencyResolver.IsIdempotentAsync(headerValue);
            if (idempotentByHeader.HasValue)
                return idempotentByHeader.Value;

            return await blackListIdempotencyResolver.IsIdempotentAsync(method, path).ConfigureAwait(false) && await iclResolver.IsIdempotentAsync(method, path).ConfigureAwait(false);
        }
    }
}