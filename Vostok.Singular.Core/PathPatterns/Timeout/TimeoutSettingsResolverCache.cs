using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal static class TimeoutSettingsResolverCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<TimeoutSettingsProvider>> Cache = new ConcurrentDictionary<(string, string), Lazy<TimeoutSettingsProvider>>();

        public static TimeoutSettingsProvider Get(IClusterClient singularClient, string environment, string service)
        {
            return Cache.GetOrAdd((environment, service), s => new Lazy<TimeoutSettingsProvider>(
                () => Create(singularClient, s.Item1, s.Item2))).Value;
        }
        private static TimeoutSettingsProvider Create(IClusterClient singularClient, string environment, string service)
        {
            return new TimeoutSettingsProvider(SettingsAliasResolverCache.Get(singularClient, environment, service),
                SettingsProviderCache.Get(singularClient, environment, service));
        }
    }
}