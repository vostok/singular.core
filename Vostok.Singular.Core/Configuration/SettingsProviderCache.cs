using System;
using System.Collections.Concurrent;
using Vostok.Clusterclient.Core;
using Vostok.Singular.Core.PathPatterns;

namespace Vostok.Singular.Core.Configuration
{
    internal static class SettingsProviderCache
    {
        private static readonly ConcurrentDictionary<(string, string), Lazy<SettingsProvider>> Cache = new ConcurrentDictionary<(string, string), Lazy<SettingsProvider>>();

        public static SettingsProvider Get(IClusterClient singularClient, string environment, string service)
        {
            return Cache.GetOrAdd((environment, service), s => new Lazy<SettingsProvider>(
                () => new SettingsProvider(singularClient, s.Item1, s.Item2))).Value;
        }
    }
}