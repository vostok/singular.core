using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class PathPatternSettingsProvider : IIdempotencySettingsProvider<SingularSettings.PathPatternSettings>
    {
        private readonly SettingsProvider settingsProvider;
        private static readonly SingularSettings.PathPatternSettings Default = new SingularSettings.PathPatternSettings();

        public PathPatternSettingsProvider(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task<SingularSettings.PathPatternSettings> GetSettingsAsync()
        {
            return await settingsProvider.GetAsync(Default).ConfigureAwait(false);
        }
    }
}