using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core;
using Vostok.Commons.Threading;
using Vostok.Commons.Time;
using Vostok.Configuration;
using Vostok.Configuration.Sources.Object;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsProvider
    {
        private readonly TimeSpan updatePeriod = TimeSpan.FromSeconds(10);
        private readonly string environment;
        private readonly string service;

        private readonly SettingsUpdater settingsUpdater;
        private readonly ObjectSource settingsSource = new ObjectSource((SingularSettings)null);
        private readonly Sync sync = new Sync();
        private readonly ILog log;

        private long? currentSettingsVersion;

        public SettingsProvider(
            IClusterClient singularClient,
            string environment,
            string service)
        {
            this.environment = environment;
            this.service = service;

            log = LogProvider.Get().ForContext("IdempotencySettingsProvider");
            settingsUpdater = new SettingsUpdater(singularClient);

            using (ExecutionContext.SuppressFlow())
                Task.Run(InitiatePeriodicUpdates);
        }

        private async Task InitiatePeriodicUpdates()
        {
            while (true)
            {
                var timeBudget = Stopwatch.StartNew();
                try
                {
                    var (settings, newVersion) = await settingsUpdater
                        .UpdateAsync(environment, service, currentSettingsVersion)
                        .ConfigureAwait(false);

                    if (!currentSettingsVersion.HasValue || newVersion > currentSettingsVersion)
                    {
                        settingsSource.Push(settings);

                        currentSettingsVersion = newVersion;
                    }
                }
                catch (Exception error)
                {
                    // TODO: падаем или нет?
                    if (!currentSettingsVersion.HasValue)
                        log.Error(error, "Failure in initial singular settings update.");
                    else
                        log.Warn("Periodical settings update routine has failed");
                }

                await Task.Delay(updatePeriod - timeBudget.Elapsed).ConfigureAwait(false);
            }
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