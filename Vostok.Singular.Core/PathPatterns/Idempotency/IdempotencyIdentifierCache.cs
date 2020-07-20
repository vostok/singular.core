using System;
using System.Collections.Concurrent;
using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal static class IdempotencyIdentifierCache
    {
        private static readonly ConcurrentDictionary<string, Lazy<IIdempotencyIdentifier>> Cache = new ConcurrentDictionary<string, Lazy<IIdempotencyIdentifier>>();

        static IdempotencyIdentifierCache()
        {
            var providerSettings = new ConfigurationProviderSettings()
                .WithErrorLogging(LogProvider.Get())
                .WithSettingsLogging(LogProvider.Get());

            var provider = new ConfigurationProvider(providerSettings);

            if (!ConfigurationProvider.TrySetDefault(provider))
                provider.Dispose();
        }

        public static IIdempotencyIdentifier Get(string serviceName)
        {
            return Cache.GetOrAdd(serviceName, s => new Lazy<IIdempotencyIdentifier>(() => Create(s))).Value;
        }

        private static IIdempotencyIdentifier Create(string serviceName)
        {
            var settingsProvider = new SettingsProvider(serviceName);
            var idempotencySignsCache = new NonIdempotencySignsCache(new NonIdempotencySignsSettingsProvider(settingsProvider));
            var iclCache = new IclCache(new IclRulesSettingsProvider(settingsProvider));
            return new IdempotencyIdentifier(
                new BlackListIdempotencyResolver(idempotencySignsCache),
                new IclResolver(iclCache)
            );
        }
    }
}