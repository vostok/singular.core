using System;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class TimeoutSettingsResolver
    {
        private readonly SettingsAliasResolver settingsAliasResolver;
        private readonly SettingsProvider settingsProvider;
        private static readonly SingularSettings EmptySettings = new();

        public TimeoutSettingsResolver(SettingsAliasResolver settingsAliasResolver, SettingsProvider settingsProvider)
        {
            this.settingsAliasResolver = settingsAliasResolver;
            this.settingsProvider = settingsProvider;
        }

        public async Task<TimeSpan> Get(string method, string path)
        {
            var rules = await settingsAliasResolver.GetPathPatternRuleAsync(method, path);

            if (rules is {TimeBudget: {} budget})
                return budget;

            var settings = await settingsProvider.GetAsync(EmptySettings).ConfigureAwait(false);
            return settings.Defaults.TimeBudget;
        } 
    }
}