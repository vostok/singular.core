using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal class PathPatternCache : ISettingsCache<PathSettings>
    {
        private static readonly SingularSettings.PathPatternSettings EmptySettings = new()
        {
            Rules = new List<PathSettingsRule>()
        };
        
        private readonly SettingsProvider settingsProvider;
        private readonly CachingTransformAsync<SingularSettings.PathPatternSettings, List<PathSettings>> cache;

        public PathPatternCache(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
            cache = new CachingTransformAsync<SingularSettings.PathPatternSettings, List<PathSettings>>(PreprocessSettings, () => GetRawValue());
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
                        PathPattern = r.PathPattern == null ? null : new Wildcard(r.PathPattern.TrimStart('/'))
                    })
                .ToList();
        }

        private async Task<SingularSettings.PathPatternSettings> GetRawValue()
        {
            return await settingsProvider.GetAsync(EmptySettings);
        }
    }
}