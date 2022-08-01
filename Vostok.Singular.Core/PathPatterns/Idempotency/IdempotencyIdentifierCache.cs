using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal static class IdempotencyIdentifierCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<IIdempotencyIdentifier>> Cache = new ConcurrentDictionary<(string, string), Lazy<IIdempotencyIdentifier>>();

        static IdempotencyIdentifierCache()
        {
            var providerSettings = new ConfigurationProviderSettings()
                .WithErrorLogging(LogProvider.Get())
                .WithSettingsLogging(LogProvider.Get());

            var provider = new ConfigurationProvider(providerSettings);

            if (!ConfigurationProvider.TrySetDefault(provider))
                provider.Dispose();
        }

        public static IIdempotencyIdentifier Get(IClusterClient singularClient, string environment, string serviceName)
        {
            return Cache.GetOrAdd((environment, serviceName), s => new Lazy<IIdempotencyIdentifier>(() => Create(singularClient, s.Item1, s.Item2))).Value;
        }

        private static IIdempotencyIdentifier Create(IClusterClient singularClient, string environment, string serviceName)
        {
            var settingsProvider = new SettingsProvider(singularClient, environment, serviceName);
            var idempotencySignsCache = new NonIdempotencySignsCache(new NonIdempotencySignsSettingsProvider(settingsProvider));
            var iclCache = new IclCache(new IclRulesSettingsProvider(settingsProvider));
            var headerIdempotencySettingsProvider = new HeaderIdempotencySettingsProvider(settingsProvider);
            return new IdempotencyIdentifier(
                new BlackListIdempotencyResolver(idempotencySignsCache),
                new IclResolver(iclCache),
                new HeaderIdempotencyResolver(headerIdempotencySettingsProvider)
            );
        }
    }
}