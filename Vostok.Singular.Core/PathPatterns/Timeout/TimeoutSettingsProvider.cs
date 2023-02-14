using System;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.PathRules;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class TimeoutSettingsProvider
    {
        private readonly PathRulesProvider pathRulesProvider;
        private readonly ISettingsProvider settingsProvider;
        private static readonly SingularSettings EmptySettings = new SingularSettings();

        public TimeoutSettingsProvider(PathRulesProvider pathRulesProvider, ISettingsProvider settingsProvider)
        {
            this.pathRulesProvider = pathRulesProvider;
            this.settingsProvider = settingsProvider;
        }

        public async Task<TimeSpan> Get(string method, string path)
        {
            var pathRule = await pathRulesProvider.Get(method, path).ConfigureAwait(false);

            if (pathRule?.TimeBudget != null)
                return pathRule.TimeBudget.Value;

            var settings = await settingsProvider.GetAsync(EmptySettings).ConfigureAwait(false);
            
            if (pathRule?.SettingsAlias == null)
                return settings.Defaults.TimeBudget;

            if (!settings.SettingsAliases.TryGetValue(pathRule.SettingsAlias, out var aliasSettings))
                return settings.Defaults.TimeBudget;

            return aliasSettings.Defaults.TimeBudget;
        }
    }
}