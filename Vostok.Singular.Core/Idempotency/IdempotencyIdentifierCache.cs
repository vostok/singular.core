using System;
using System.Collections.Concurrent;
using Vostok.Configuration;
using Vostok.Configuration.Logging;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Idempotency.Identifier;
using Vostok.Singular.Core.Identifier;

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
            var idempotencySignsCache = new NonIdempotencySignsCache(new IdempotencySignsProvider(serviceName));
            return new IdempotencyIdentifier(
                new BlackListIdempotencyResolver(idempotencySignsCache),
                new IclResolver(new IclCache(new IclSettingsProvider(serviceName)))
            );
        }
    }
}