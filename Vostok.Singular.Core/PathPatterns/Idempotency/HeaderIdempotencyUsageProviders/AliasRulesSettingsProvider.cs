using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotencyUsageProviders
{
    internal class AliasRulesSettingsProvider : IIdempotencySettingsProvider<Dictionary<string, SingularSettings>>
    {
        private readonly SettingsProvider settingsProvider;
        private static readonly Dictionary<string, SingularSettings> Default = new Dictionary<string, SingularSettings>();

        public AliasRulesSettingsProvider(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task<Dictionary<string, SingularSettings>> GetSettingsAsync()
        {
            return await settingsProvider.GetAsync(Default).ConfigureAwait(false);
        }
    }
}