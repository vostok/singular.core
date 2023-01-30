namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal class SettingsAliasIdentifierFactory : IdentifierFactory<SettingsAliasResolver>
    {
        protected override SettingsAliasResolver Create(ISettingsProvider settingsProvider)
        {
            return new SettingsAliasResolver(new PathPatternCache(settingsProvider));
        }
    }
}