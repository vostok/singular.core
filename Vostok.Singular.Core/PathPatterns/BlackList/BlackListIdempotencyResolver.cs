using System;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal class BlackListIdempotencyResolver : IBlackListIdempotencyResolver
    {
        private readonly ISettingsCache<NonIdempotencySign> nonIdempotencySignsCache;

        public BlackListIdempotencyResolver(ISettingsCache<NonIdempotencySign> nonIdempotencySignsCache)
        {
            this.nonIdempotencySignsCache = nonIdempotencySignsCache;
        }

        public async Task<bool> IsIdempotent(string method, string path)
        {
            var signs = await nonIdempotencySignsCache.GetAsync().ConfigureAwait(false);
            if (signs.Count > 0 && path.StartsWith("/"))
                path = path.TrimStart('/');

            foreach (var sign in signs)
            {
                if (sign.Method == null || sign.PathPattern == null)
                    continue;

                if (!string.Equals(sign.Method, method, StringComparison.OrdinalIgnoreCase) || path == null)
                    continue;

                if (sign.PathPattern.IsMatch(path))
                    return false;
            }

            return true;
        }
    }
}