using System.Collections.Generic;
using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;

namespace Vostok.Singular.Core.PathPatterns.BlackList
{
    internal class NonIdempotencySignsSettingsProvider : INonIdempotencySignsSettingsProvider
    {
        private static readonly NonIdempotencyServiceSettings EmptySigns = new NonIdempotencyServiceSettings
        {
            NonIdempotencySigns = new NonIdempotencySignsSettings
            {
                Signs = new List<NonIdempotencySignSettings>(0)
            }
        };

        private readonly ISettingsProvider settingsProvider;

        public NonIdempotencySignsSettingsProvider(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async ValueTask<NonIdempotencySignsSettings> GetAsync()
        {
            return (await settingsProvider.GetAsync(EmptySigns).ConfigureAwait(false)).NonIdempotencySigns;
        }
    }
}