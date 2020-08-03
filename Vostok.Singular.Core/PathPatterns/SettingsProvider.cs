using Vostok.ClusterConfig.Client;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsProvider
    {
        private readonly IConfigurationSource source;
        private readonly IConfigurationSource zoneSource;
        private readonly IConfigurationSource combinedSource;
        private readonly string servicePath;
        private readonly string zonePath;

        public SettingsProvider(
            string zone,
            string serviceName,
            string zonesConfigurationPathPrefix = SingularClientConstants.ZonesConfigurationNamePrefix,
            string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{serviceName}.json";
            zonePath = $"{zonesConfigurationPathPrefix}{zone}/singular.config.json";
            zoneSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, zonePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });

            source = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
            combinedSource = zoneSource.CombineWith(source);
        }

        public T Get<T>(T defaultValue)
        {
            var resultNumber = 0;
            if (ClusterConfigClient.Default.Get(zonePath) != null)
                resultNumber += 1;
            if (ClusterConfigClient.Default.Get(servicePath) != null)
                resultNumber += 2;
            switch (resultNumber)
            {
                case 0:
                    return defaultValue;
                case 1:
                    return ConfigurationProvider.Default.Get<T>(zoneSource);
                case 2:
                    return ConfigurationProvider.Default.Get<T>(source);
                case 3:
                    return ConfigurationProvider.Default.Get<T>(combinedSource);
            }

            return ConfigurationProvider.Default.Get<T>(source);
        }
    }
}