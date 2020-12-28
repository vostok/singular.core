using System.Threading.Tasks;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Commons.Threading;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsProvider
    {
        private readonly IClusterConfigClient clusterConfigClient;
        private readonly IConfigurationSource serviceSource;
        private readonly IConfigurationSource environmentSource;
        private readonly IConfigurationSource combinedSource;
        private readonly string servicePath;
        private readonly string environmentPath;

        private readonly Sync sync = new Sync();

        public SettingsProvider(
            IClusterConfigClient clusterConfigClient,
            string environment,
            string application,
            string environmentsConfigurationPathPrefix = SingularClientConstants.EnvironmentsConfigurationNamePrefix,
            string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            this.clusterConfigClient = clusterConfigClient;
            servicePath = $"{configurationPathPrefix}{application}.json";
            environmentPath = $"{environmentsConfigurationPathPrefix}{environment}/{SingularConstants.SingularSettingsFileName}";
            environmentSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(clusterConfigClient, environmentPath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });

            serviceSource = new ClusterConfigSource(
                new ClusterConfigSourceSettings(clusterConfigClient, servicePath)
                {
                    ValuesParser = (value, path) => JsonConfigurationParser.Parse(value)
                });
            combinedSource = environmentSource.CombineWith(serviceSource);
        }

        public async Task<T> GetAsync<T>(T defaultValue)
        {
            if (sync.WaitTask.Task.IsCompleted)
                return Get(defaultValue);

            if (sync.CreateStarted.TrySetTrue())
            {
                try
                {
                    return Get(defaultValue);
                }
                finally
                {
                    sync.WaitTask.SetResult(true);
                }
            }

            await sync.WaitTask.Task.ConfigureAwait(false);
            return Get(defaultValue);
        }

        private T Get<T>(T defaultValue)
        {
            var environmentSettingsExists = clusterConfigClient.Get(environmentPath) != null;
            var serviceSettingsExists = clusterConfigClient.Get(servicePath) != null;

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

        private class Sync
        {
            public readonly TaskCompletionSource<bool> WaitTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public readonly AtomicBoolean CreateStarted = new AtomicBoolean(false);
        }
    }
}