using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core;
using Vostok.Configuration.Sources.Manual;
using Vostok.Logging.Abstractions;

namespace Vostok.Singular.Core.Configuration
{
    internal class SingularConfigurationSource : ManualFeedSource
    {
        private readonly TimeSpan updatePeriod = TimeSpan.FromSeconds(10);

        private readonly string environment;
        private readonly string service;
        private readonly SettingsUpdater settingsUpdater;
        private readonly ILog log;

        public SingularConfigurationSource(
            string environment,
            string service,
            IClusterClient singularClient,
            ILog log = null)
        {
            this.environment = environment;
            this.service = service;
            settingsUpdater = new SettingsUpdater(singularClient);
            this.log = (log ?? LogProvider.Get()).ForContext($"SingularConfigurationSource({service} in {environment})");

            using (ExecutionContext.SuppressFlow())
                Task.Run(InitiatePeriodicUpdates);
        }

        private async Task InitiatePeriodicUpdates()
        {
            var previousResult = (SettingsUpdaterResult)null;
            while (true)
            {
                var timeBudget = Stopwatch.StartNew();
                try
                {
                    var actualResult = await settingsUpdater
                        .UpdateAsync(environment, service, previousResult)
                        .ConfigureAwait(false);

                    if (previousResult == null || actualResult.Changed)
                        Push(actualResult.Settings);

                    previousResult = actualResult;
                }
                catch (Exception error)
                {
                    if (previousResult == null)
                    {
                        log.Error(error, "Failure in initial singular settings update.");
                        Push(null, error);
                    }
                    else
                        log.Warn(error, "Periodical settings update routine has failed.");
                }

                var waitTime = updatePeriod - timeBudget.Elapsed;
                
                await Task.Delay(waitTime.TotalMilliseconds > 0 ? waitTime : TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
            }
        }
    }
}