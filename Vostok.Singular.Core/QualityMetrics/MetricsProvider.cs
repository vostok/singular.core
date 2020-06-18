using System;
using Vostok.Metrics;
using Vostok.Metrics.Grouping;
using Vostok.Metrics.Models;
using Vostok.Metrics.Primitives.Counter;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal class MetricsProvider
    {
        private const string Version = "v0";
        private readonly IMetricGroup1<ICounter> requestsWithoutHost;

        public MetricsProvider(IMetricContext metricContext, string environment, string clientName)
        {
            var sendZeroValuesCounterConfig = new CounterConfig
            {
                ScrapePeriod = TimeSpan.FromMinutes(1),
                SendZeroValues = false
            };

            var metricsTags = new MetricTags(
                new MetricTag("project", SingularConstants.ProjectName),
                new MetricTag("environment", environment),
                new MetricTag("application", SingularConstants.ServiceName),
                new MetricTag("cluster", SingularConstants.DefaultCluster),
                new MetricTag("client", clientName),
                new MetricTag("version", Version)
            );

            var metricsContext = metricContext.WithTags(metricsTags);
            requestsWithoutHost = metricsContext.CreateCounter("singularClient", "reason", sendZeroValuesCounterConfig);
        }

        public void RecordRequest(ResultReason reason)
        {
            requestsWithoutHost.For(reason.ToString()).Add(1);
        }
    }
}