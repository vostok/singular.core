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
        private readonly IConfigurationSource environmentSource;
        private readonly IConfigurationSource combinedSource;
        private readonly string servicePath;
        private readonly string environmentPath;

        public SettingsProvider(
            string environment,
            string serviceName,
            string environmentsConfigurationPathPrefix = SingularClientConstants.EnvironmentsConfigurationNamePrefix,
            string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{serviceName}.json";
            environmentPath = $"{environmentsConfigurationPathPrefix}{environment}/singular.config.json";
            environmentSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, environmentPath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });

            source = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
            combinedSource = environmentSource.CombineWith(source);
        }

        public T Get<T>(T defaultValue)
        {
            var resultNumber = 0;
            if (ClusterConfigClient.Default.Get(environmentPath) != null)
                resultNumber += 1;
            if (ClusterConfigClient.Default.Get(servicePath) != null)
                resultNumber += 2;
            switch (resultNumber)
            {
                case 0:
                    return defaultValue;
                case 1:
                    return ConfigurationProvider.Default.Get<T>(environmentSource);
                case 2:
                    return ConfigurationProvider.Default.Get<T>(source);
                case 3:
                    return ConfigurationProvider.Default.Get<T>(combinedSource);
            }

            return ConfigurationProvider.Default.Get<T>(source);
        }
    }
}