using Vostok.Singular.Core.Idempotency.BlackList;
using Vostok.Singular.Core.Idempotency.Icl;

namespace Vostok.Singular.Core.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly BlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IclResolver iclResolver;

        public IdempotencyIdentifier(
            BlackListIdempotencyResolver blackListIdempotencyResolver,
            IclResolver iclResolver
        )
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
        }

        public bool IsIdempotent(string method, string path)
        {
            return blackListIdempotencyResolver.IsIdempotent(method, path) && iclResolver.IsIdempotent(method, path);
        }
    }
}