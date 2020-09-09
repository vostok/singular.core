using System.Threading.Tasks;
using Vostok.ClusterConfig.Client;
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
        private readonly IConfigurationSource serviceSource;
        private readonly IConfigurationSource environmentSource;
        private readonly IConfigurationSource combinedSource;
        private readonly string servicePath;
        private readonly string environmentPath;

        private readonly Sync sync = new Sync();

        public SettingsProvider(
            string environment,
            string application,
            string environmentsConfigurationPathPrefix = SingularClientConstants.EnvironmentsConfigurationNamePrefix,
            string configurationPathPrefix = SingularClientConstants.ServicesConfigurationNamePrefix)
        {
            servicePath = $"{configurationPathPrefix}{application}.json";
            environmentPath = $"{environmentsConfigurationPathPrefix}{environment}/{SingularConstants.SingularSettingsFileName}";
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

        public async Task<T> GetAsync<T>(T defaultValue)
        {
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

            await sync.WaitTask.Task;
            return Get(defaultValue);
        }

        private T Get<T>(T defaultValue)
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

        private class Sync
        {
            public readonly TaskCompletionSource<bool> WaitTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public readonly AtomicBoolean CreateStarted = new AtomicBoolean(false);
        }
    }
}