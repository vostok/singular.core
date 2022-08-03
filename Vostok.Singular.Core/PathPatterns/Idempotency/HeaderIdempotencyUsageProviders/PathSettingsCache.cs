using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class PathSettingsCache
    {
        private readonly CachingTransformAsync<SingularSettings.PathPatternSettings, List<PathSettings>> cache;

        public PathSettingsCache(IIdempotencySettingsProvider<SingularSettings.PathPatternSettings> settingsProvider)
        {
            cache = new CachingTransformAsync<SingularSettings.PathPatternSettings, List<PathSettings>>(PreprocessSettings, settingsProvider.GetSettingsAsync);
        }

        public async Task<List<PathSettings>> GetAsync()
        {
            return await cache.GetAsync();
        }

        private static List<PathSettings> PreprocessSettings(SingularSettings.PathPatternSettings settings)
        {
            return settings
                .Rules
                .Select(
                    r => new PathSettings
                    {
                        Method = r.Method,
                        SettingsAlias = r.SettingsAlias,
                        TimeBudget = r.TimeBudget,
                        StrategySettings = r.StrategySettings,
                        RedirectSettings = r.RedirectSettings,
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern.TrimStart('/')),
                        OverrideHeader = r.OverrideHeader
                    })
                .ToList();
        }
    }
}