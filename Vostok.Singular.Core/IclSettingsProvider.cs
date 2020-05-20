using System.Collections.Generic;
using Vostok.ClusterConfig.Client;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;
using Vostok.Singular.Core.Settings;

namespace Vostok.Singular.Core
{
    internal class IclSettingsProvider : IIclSettingsProvider
    {
        private readonly IConfigurationSource source;
        private readonly string servicePath;

        public IclSettingsProvider(string serviceName, string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{serviceName}.json";
            source = new ClusterConfigSource(
                new ClusterConfigSourceSettings(ClusterConfigClient.Default, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
        }

        public List<IdempotencyRuleSetting> Get()
        {
            if (ClusterConfigClient.Default.Get(servicePath) == null)
                return new List<IdempotencyRuleSetting>(0);

            return ConfigurationProvider.Default.Get<IdempotencyControlListSetting>(source).Rules;
        }
    }
}