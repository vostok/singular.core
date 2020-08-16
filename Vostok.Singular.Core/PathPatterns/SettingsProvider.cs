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
        private readonly IConfigurationSource serviceSource;
        private readonly IConfigurationSource environmentSource;
        private readonly IConfigurationSource combinedSource;
        private readonly string servicePath;
        private readonly string environmentPath;

        public SettingsProvider(
            string environment,
            string application,
            string environmentsConfigurationPathPrefix = SingularClientConstants.EnvironmentsConfigurationNamePrefix,
            string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{application}.json";
            environmentPath = $"{environmentsConfigurationPathPrefix}{environment}/singular.config.json";
            environmentSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, environmentPath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });

            serviceSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
            combinedSource = environmentSource.CombineWith(serviceSource);
        }

        public T Get<T>(T defaultValue)
        {
            var environmentSettingsExists = ClusterConfigClient.Default.Get(environmentPath) != null;
            var serviceSettingsExists = ClusterConfigClient.Default.Get(servicePath) != null;

            if (!environmentSettingsExists && !serviceSettingsExists)
                return defaultValue;

            IConfigurationSource resultSource;

            if (environmentSettingsExists && serviceSettingsExists)
                resultSource = combinedSource;
            else if (environmentSettingsExists)
                resultSource = environmentSource;
            else
                resultSource = serviceSource;

            return ConfigurationProvider.Default.Get<T>(resultSource);
        }
    }
}