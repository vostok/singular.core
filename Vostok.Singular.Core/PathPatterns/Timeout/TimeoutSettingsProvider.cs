using System;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class TimeoutSettingsProvider
    {
        private readonly SettingsAliasProvider settingsAliasProvider;
        private readonly ISettingsProvider settingsProvider;
        private static readonly SingularSettings EmptySettings = new SingularSettings();

        public TimeoutSettingsProvider(SettingsAliasProvider settingsAliasProvider, ISettingsProvider settingsProvider)
        {
            this.settingsAliasProvider = settingsAliasProvider;
            this.settingsProvider = settingsProvider;
        }

        public async Task<TimeSpan> Get(string method, string path)
        {
            var pathRule = await settingsAliasProvider.Get(method, path).ConfigureAwait(false);

            if (pathRule?.TimeBudget != null)
                return pathRule.TimeBudget.Value;

            var settings = await settingsProvider.GetAsync(EmptySettings).ConfigureAwait(false);
            
            return settings.Defaults.TimeBudget;
        } 
    }
}