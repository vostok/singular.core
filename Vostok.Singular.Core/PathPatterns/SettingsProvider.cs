using System.Threading.Tasks;
using Vostok.Clusterclient.Core;
using Vostok.Commons.Threading;
using Vostok.Configuration;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsProvider : ISettingsProvider
    {
        private readonly SingularConfigurationSource settingsSource;
        private readonly Sync sync = new Sync();

        public SettingsProvider(
            IClusterClient singularClient,
            string environment,
            string service)
        {
            settingsSource = new SingularConfigurationSource(environment, service, singularClient, LogProvider.Get());
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
            var value = ConfigurationProvider.Default.Get<T>(settingsSource);
            return value == null ? defaultValue : value;
        }

        private class Sync
        {
            public readonly TaskCompletionSource<bool> WaitTask = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public readonly AtomicBoolean CreateStarted = new AtomicBoolean(false);
        }
    }
}