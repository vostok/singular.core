using System.Linq;
using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class PathSettingsRulesResolver
    {
        private readonly PathSettingsCache pathSettingsCache;
        private readonly AliasRulesResolver aliasRulesResolver;

        public PathSettingsRulesResolver(PathSettingsCache pathSettingsCache, AliasRulesResolver aliasRulesResolver)
        {
            this.pathSettingsCache = pathSettingsCache;
            this.aliasRulesResolver = aliasRulesResolver;
        }

        public async Task<bool> CanUseHeader(string method, string path)
        {
            var pathSettings = await pathSettingsCache.GetAsync();

            var pathSetting = pathSettings.FirstOrDefault(x => PathPatternRuleMatcher.IsMatch(x, method, path));

            if (pathSetting is null)
                return false;

            if (pathSetting.OverrideHeader)
                return true;

            if (string.IsNullOrEmpty(pathSetting.SettingsAlias))
                return pathSetting.OverrideHeader;

            return await aliasRulesResolver.CanUseHeader(pathSetting.SettingsAlias);
        }
    }
}