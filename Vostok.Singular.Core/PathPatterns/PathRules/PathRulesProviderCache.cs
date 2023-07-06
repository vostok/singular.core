using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.PathRules
{
    internal static class  PathRulesProviderCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<PathRulesProvider>> Cache = new ConcurrentDictionary<(string, string), Lazy<PathRulesProvider>>();

        public static PathRulesProvider Get(IClusterClient singularClient, string environment, string service)
        {
            return Cache.GetOrAdd((environment, service),
                    s => new Lazy<PathRulesProvider>(
                        () => Create(singularClient, s.Item1, s.Item2)))
                .Value;
        }

        private static PathRulesProvider Create(IClusterClient singularClient, string environment, string service)
        {
            return new PathRulesProvider(new PathRulesCache(
                new SingularServiceSettingsProvider(SettingsProviderCache.Get(singularClient, environment, service))));
        }
    }
}