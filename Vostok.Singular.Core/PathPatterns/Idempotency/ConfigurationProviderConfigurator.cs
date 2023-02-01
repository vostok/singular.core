using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal static class ConfigurationProviderConfigurator
    {
        public static void SetupLoggingAndSetDefaultProvider()
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