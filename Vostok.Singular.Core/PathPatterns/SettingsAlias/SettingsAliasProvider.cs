using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal class SettingsAliasProvider
    {
        private readonly PathPatternCache cache;

        public SettingsAliasProvider(PathPatternCache cache)
        {
            this.cache = cache;
        }

        public async Task<PathSettings> Get(string method, string path)
        {
            var rules = await cache.GetAsync().ConfigureAwait(false);

            if (rules?.Count > 0 && path.StartsWith("/"))
                path = path.TrimStart('/');

            return rules?.FirstOrDefault(r => PathPatternRuleMatcher.IsMatch(r, method, path));
        }
    }
}