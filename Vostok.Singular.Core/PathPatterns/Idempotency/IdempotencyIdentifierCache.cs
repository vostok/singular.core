using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal static class IdempotencyIdentifierCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<IIdempotencyIdentifier>> Cache = new ConcurrentDictionary<(string, string), Lazy<IIdempotencyIdentifier>>();

        public static IIdempotencyIdentifier Get(IClusterClient singularClient, string environment, string serviceName)
        {
            return Cache.GetOrAdd((environment, serviceName), s => new Lazy<IIdempotencyIdentifier>(() => Create(singularClient, s.Item1, s.Item2))).Value;
        }

        private static IIdempotencyIdentifier Create(IClusterClient singularClient, string environment, string serviceName)
        {
            var settingsProvider = SettingsProviderCache.Get(singularClient, environment, serviceName);
            var idempotencySignsCache = new NonIdempotencySignsCache(new NonIdempotencySignsSettingsProvider(settingsProvider));
            var iclCache = new IclCache(new IclRulesSettingsProvider(settingsProvider));
            
            return new IdempotencyIdentifier(
                new BlackListIdempotencyResolver(idempotencySignsCache),
                new IclResolver(iclCache)
            );
        }
    }
}