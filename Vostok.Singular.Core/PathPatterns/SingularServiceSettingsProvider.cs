using System.Threading.Tasks;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SingularServiceSettingsProvider : IServiceSettingsProvider
    {
        private readonly ISettingsProvider settingsProvider;
        private static readonly SingularSettings EmptySettings = new SingularSettings();

        public SingularServiceSettingsProvider(ISettingsProvider settingsProvider)
        {
            this.settingsProvider = settingsProvider;
        }
        
        public Task<SingularSettings> Get()
        {
            return settingsProvider.GetAsync(EmptySettings);
        }
    }
}