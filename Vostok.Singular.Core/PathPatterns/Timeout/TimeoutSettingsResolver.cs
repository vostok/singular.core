using System;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class TimeoutSettingsResolver
    {
        private readonly SettingsAliasResolver settingsAliasResolver;
        private readonly ISettingsProvider settingsProvider;
        private static readonly SingularSettings EmptySettings = new SingularSettings();

        public TimeoutSettingsResolver(SettingsAliasResolver settingsAliasResolver, ISettingsProvider settingsProvider)
        {
            this.settingsAliasResolver = settingsAliasResolver;
            this.settingsProvider = settingsProvider;
        }

        public async Task<TimeSpan?> Get(string method, string path)
        {
            var pathRule = await settingsAliasResolver.GetPathPatternRuleAsync(method, path).ConfigureAwait(false);

            if (pathRule?.TimeBudget != null)
                return pathRule.TimeBudget.Value;

            var settings = await settingsProvider.GetAsync(EmptySettings).ConfigureAwait(false);
            if (settings.Defaults.TimeBudget == EmptySettings.Defaults.TimeBudget)
                return null;
            
            return settings.Defaults.TimeBudget;
        } 
    }
}