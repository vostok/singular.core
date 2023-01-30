﻿using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal class SettingsAliasResolver
    {
        private readonly ISettingsCache<PathSettings> cache;

        public SettingsAliasResolver(ISettingsCache<PathSettings> cache)
        {
            this.cache = cache;
        }

        public async Task<PathSettings> GetPathPatternRuleAsync(string method, string path)
        {
            var rules = await cache.GetAsync().ConfigureAwait(false);

            if (rules?.Count > 0 && path.StartsWith("/"))
                path = path.TrimStart('/');

            return rules?.FirstOrDefault(r => PathPatternRuleMatcher.IsMatch(r, method, path));
        }
    }
}