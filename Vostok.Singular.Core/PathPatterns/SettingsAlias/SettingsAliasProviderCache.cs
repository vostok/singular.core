using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.SettingsAlias
{
    internal static class SettingsAliasProviderCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<SettingsAliasProvider>> Cache = new ConcurrentDictionary<(string, string), Lazy<SettingsAliasProvider>>();

        public static SettingsAliasProvider Get(IClusterClient singularClient, string environment, string service)
        {
            return Cache.GetOrAdd((environment, service),
                    s => new Lazy<SettingsAliasProvider>(
                        () => Create(singularClient, s.Item1, s.Item2)))
                .Value;
        }

        private static SettingsAliasProvider Create(IClusterClient singularClient, string environment, string service)
        {
            return new SettingsAliasProvider(new PathPatternCache(
                new SingularServiceSettingsProvider(SettingsProviderCache.Get(singularClient, environment, service))));
        }
    }
}