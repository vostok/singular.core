using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SingularServiceSettingsProvider : IServiceSettingsProvider
    {
        private static readonly SingularSettings EmptySettings = new SingularSettings();
        private readonly ISettingsProvider settingsProvider;

        public SingularServiceSettingsProvider(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }

        public async ValueTask<SingularSettings> Get()
        {
            return await settingsProvider.GetAsync(EmptySettings).ConfigureAwait(false);
        }
    }
}