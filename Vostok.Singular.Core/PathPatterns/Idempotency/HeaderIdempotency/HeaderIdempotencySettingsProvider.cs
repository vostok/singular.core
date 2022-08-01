using System.Threading.Tasks;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    internal class HeaderIdempotencySettingsProvider : IHeaderIdempotencySettingsProvider
    {
        private static readonly IdempotencyHeaderSettings Default = new IdempotencyHeaderSettings
        {
            OverrideHeader = false
        };

        private readonly SettingsProvider settingsProvider;

        public HeaderIdempotencySettingsProvider(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task<IdempotencyHeaderSettings> GetAsync()
        {
            return await settingsProvider.GetAsync(Default).ConfigureAwait(false);
        }
    }
}