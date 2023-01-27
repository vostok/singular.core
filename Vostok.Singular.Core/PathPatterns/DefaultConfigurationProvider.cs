using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;

namespace Vostok.Singular.Core.PathPatterns
{
    internal static class DefaultConfigurationProvider
    {
        public static void Set()
        {
            var providerSettings = new ConfigurationProviderSettings()
                .WithErrorLogging(LogProvider.Get())
                .WithSettingsLogging(LogProvider.Get());

            var provider = new ConfigurationProvider(providerSettings);

            if (!ConfigurationProvider.TrySetDefault(provider))
                provider.Dispose();
        }
    }
}