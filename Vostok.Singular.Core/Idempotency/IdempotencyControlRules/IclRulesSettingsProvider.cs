using System.Collections.Generic;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal class IclRulesSettingsProvider : IIclRulesSettingsProvider
    {
        private readonly SettingsProvider settingsProvider;

        private static readonly IclRulesServiceSettings EmptyRules = new IclRulesServiceSettings
        {
            IclRulesSettings = new IclRulesSettings
            {
                Rules = new List<IdempotencyRuleSetting>(0)
            }
        };
        public IclRulesSettingsProvider(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public IclRulesSettings Get()
        {
            return settingsProvider.Get(EmptyRules).IclRulesSettings;
        }
    }
}