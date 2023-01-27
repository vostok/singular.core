namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal class SettingsAliasIdentifierFactory : IdentifierFactory<SettingsAliasResolver>
    {
        protected override SettingsAliasResolver Create(ISettingsProvider settingsProvider, string zone, string service)
        {
            return new SettingsAliasResolver(new PathPatternCache(settingsProvider));
        }
    }
}