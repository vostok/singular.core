using System;
using System.Collections.Concurrent;
using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Idempotency.BlackList;
using Vostok.Singular.Core.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.Idempotency
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
            //CR: (deniaa) The complicated creation of the first argument was given a separate line.
            //CR: (deniaa) But the second one (absolutely same) was not.
            //CR: (deniaa) Where is your sense of perfectionism?
            return new IdempotencyIdentifier(
                new BlackListIdempotencyResolver(idempotencySignsCache),
                new IclResolver(new IclCache(new IclRulesSettingsProvider(settingsProvider)))
            );
        }
    }
}