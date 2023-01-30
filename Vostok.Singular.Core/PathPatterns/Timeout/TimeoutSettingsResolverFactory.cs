using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class TimeoutSettingsResolverFactory : IdentifierFactory<TimeoutSettingsResolver>
    {
        private readonly SettingsAliasResolver settingsAliasResolver;

        public TimeoutSettingsResolverFactory(SettingsAliasResolver settingsAliasResolver)
        {
            this.settingsAliasResolver = settingsAliasResolver;
        }
        
        protected override TimeoutSettingsResolver Create(ISettingsProvider settingsProvider)
        {
            return new TimeoutSettingsResolver(settingsAliasResolver, settingsProvider);
        }
    }
}