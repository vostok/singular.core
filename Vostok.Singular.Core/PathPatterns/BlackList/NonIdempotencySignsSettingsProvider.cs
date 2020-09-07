using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
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

        public Task<NonIdempotencySignsSettings> Get()
        {
            return Task.FromResult(settingsProvider.Get(EmptySigns).NonIdempotencySigns);
        }
    }
}