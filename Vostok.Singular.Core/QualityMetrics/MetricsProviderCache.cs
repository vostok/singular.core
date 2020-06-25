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
            // CR: Сейчас, если одновременно налетит много запросов в один env от одного client, то new MetricsProvider() вызовется несколько раз,
            // CR: а там внутри есть IDisposable штуки, но все они вроде бы кэшируются, так что все ок (можно еще раз убедиться на всякий случай).
            return Cache.GetOrAdd(Tuple.Create(metricContext, environment, clientName), t => new MetricsProvider(t.Item1, t.Item2, t.Item3));
        }
    }
}