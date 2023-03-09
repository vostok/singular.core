using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules
{
    internal class IclRulesSettingsProvider : IIclRulesSettingsProvider
    {
        private static readonly IclRulesServiceSettings EmptyRules = new IclRulesServiceSettings
        {
            IdempotencySettings = new IdempotencySettings
            {
                Rules = new List<IdempotencyRuleSetting>(0)
            }
        };

        private readonly ISettingsProvider settingsProvider;

        public IclRulesSettingsProvider(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async Task<IdempotencySettings> GetAsync()
        {
            return (await settingsProvider.GetAsync(EmptyRules).ConfigureAwait(false)).IdempotencySettings;
        }
    }
}