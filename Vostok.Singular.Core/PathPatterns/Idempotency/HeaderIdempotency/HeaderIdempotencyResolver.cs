using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal class HeaderIdempotencyResolver : IHeaderIdempotencyResolver
    {
        private readonly IIdempotencyHeaderUsageProvider idempotencyHeaderUsageProvider;

        public HeaderIdempotencyResolver(IIdempotencyHeaderUsageProvider idempotencyHeaderUsageProvider)
        {
            this.idempotencyHeaderUsageProvider = idempotencyHeaderUsageProvider;
        }

        public async Task<bool?> IsIdempotentAsync(string method, string path, string headerValue)
        {
            var canUseHeader = await idempotencyHeaderUsageProvider.CanUseHeader(method, path);

            if (!canUseHeader)
                return null;

            if (!bool.TryParse(headerValue, out var isIdempotent))
                return null;

            return isIdempotent;
        }
    }
}