using System.Collections.Generic;
using Vostok.Singular.Core.Idempotency.BlackList.Settings;

namespace Vostok.Singular.Core.Idempotency.BlackList
{
    internal class NonIdempotencySignsSettingsProvider : INonIdempotencySignsSettingsProvider
    {
        private readonly SettingsProvider settingsProvider;

        private static readonly NonIdempotencyServiceSettings EmptySigns = new NonIdempotencyServiceSettings
        {
            NonIdempotencySigns = new NonIdempotencySignsSettings
            {
                Signs = new List<NonIdempotencySignSettings>(0)
            }
        };
        public NonIdempotencySignsSettingsProvider(SettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public NonIdempotencySignsSettings Get()
        {
            return settingsProvider.Get(EmptySigns).NonIdempotencySigns;
        }
    }
}