using Vostok.ClusterConfig.Client;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsProvider
    {
        private readonly IConfigurationSource source;
        private readonly string servicePath;

        public SettingsProvider(string serviceName, string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{serviceName}.json";
            source = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
        }

        public T Get<T>(T defaultValue)
        {
            if (ClusterConfigClient.Default.Get(servicePath) == null)
                return defaultValue;

            return ConfigurationProvider.Default.Get<T>(source);
        }
    }
}