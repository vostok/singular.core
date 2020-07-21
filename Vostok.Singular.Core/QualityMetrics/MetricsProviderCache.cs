using System;
using System.Collections.Concurrent;
using Vostok.Metrics;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal static class MetricsProviderCache
    {
        private static readonly ConcurrentDictionary<Tuple<IMetricContext, string, string>, MetricsProvider> Cache = new ConcurrentDictionary<Tuple<IMetricContext, string, string>, MetricsProvider>();

        public static MetricsProvider Get(IMetricContext metricContext, string environment, string clientName)
        {
            return Cache.GetOrAdd(Tuple.Create(metricContext, environment, clientName), t => new MetricsProvider(t.Item1, t.Item2, t.Item3));
        }
    }
}