using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Singular.Core.QualityMetrics;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    internal class ClusterResultsAnalyzer_Tests
    {
        private Request request;
        private Uri anyUri;
        private Headers cleanHeaders;
        private Headers xSingularBackendHeaders;
        private Headers xSingularThrottlingTriggerHeaders;

        [SetUp]
        public void SetUp()
        {
            anyUri = new Uri("http://tmp.com");
            request = new Request("GET", anyUri);
            cleanHeaders = new Headers(2).Set("test", "true");
            xSingularBackendHeaders = cleanHeaders.Set(SingularHeaders.Backend, "true");
            xSingularThrottlingTriggerHeaders = cleanHeaders.Set(SingularHeaders.IsSingularThrottlingTrigger, "true");
        }

        [TestCase(ClusterResultStatus.Success, ResultReason.Backend)]
        [TestCase(ClusterResultStatus.TimeExpired, ResultReason.TimeExpired)]
        [TestCase(ClusterResultStatus.ReplicasNotFound, ResultReason.SingularReplicasNotFound)]
        [TestCase(ClusterResultStatus.ReplicasExhausted, ResultReason.Backend)]
        [TestCase(ClusterResultStatus.IncorrectArguments, ResultReason.Backend)]
        [TestCase(ClusterResultStatus.UnexpectedException, ResultReason.UnexpectedException)]
        [TestCase(ClusterResultStatus.Canceled, ResultReason.Backend)]
        [TestCase(ClusterResultStatus.Throttled, ResultReason.Backend)]
        public void Should_choose_correct_verdict_when_answers_is_empty_depends_on_status(ClusterResultStatus status, ResultReason result)
        {
            var emptyReplicaResults = new List<ReplicaResult>();
            var clusterResult = new ClusterResult(status, emptyReplicaResults, null, request);
            ClusterResultsAnalyzer.FindResultSource(clusterResult).Should().Be(result);
        }

        [TestCase(true, ResultReason.SingularThrottling)]
        [TestCase(false, ResultReason.Backend)]
        public void Should_choose_correct_verdict_for_throttling_depends_on_header(bool withSingularThrottlingTriggerHeader, ResultReason result)
        {
            var replicaResults = new List<ReplicaResult>
            {
                CreateRejectReplicaResult(
                    ResponseCode.TooManyRequests,
                    withSingularThrottlingTriggerHeader ? xSingularThrottlingTriggerHeaders : cleanHeaders
                )
            };

            var clusterResult = new ClusterResult(ClusterResultStatus.ReplicasExhausted, replicaResults, null, request);
            ClusterResultsAnalyzer.FindResultSource(clusterResult).Should().Be(result);
        }

        [TestCase(ResponseCode.RequestTimeout, ResultReason.RequestTimeout)]
        [TestCase(ResponseCode.ConnectFailure, ResultReason.ConnectFailure)]
        [TestCase(ResponseCode.ReceiveFailure, ResultReason.ReceiveFailure)]
        [TestCase(ResponseCode.SendFailure, ResultReason.SendFailure)]
        [TestCase(ResponseCode.InternalServerError, ResultReason.InternalServerError)]
        [TestCase(ResponseCode.BadGateway, ResultReason.Backend)]
        [TestCase(ResponseCode.ServiceUnavailable, ResultReason.ServiceUnavailable)]
        [TestCase(ResponseCode.ProxyTimeout, ResultReason.ProxyTimeout)]
        [TestCase(ResponseCode.Unknown, ResultReason.Unknown)]
        [TestCase(ResponseCode.UnknownFailure, ResultReason.UnknownFailure)]
        [TestCase(ResponseCode.StreamReuseFailure, ResultReason.StreamReuseFailure)]
        [TestCase(ResponseCode.StreamInputFailure, ResultReason.StreamInputFailure)]
        public void Should_choose_correct_verdict_for_one_replica_with_exhausted_status_depends_on_code_and_header(ResponseCode responseCode, ResultReason result)
        {
            var replicaResults = new List<ReplicaResult>
            {
                CreateRejectReplicaResult(responseCode, cleanHeaders)
            };
            var clusterResult = new ClusterResult(ClusterResultStatus.ReplicasExhausted, replicaResults, null, request);
            ClusterResultsAnalyzer.FindResultSource(clusterResult).Should().Be(result);

            var backendReplicaResults = new List<ReplicaResult>
            {
                CreateRejectReplicaResult(responseCode, xSingularBackendHeaders)
            };
            clusterResult = new ClusterResult(ClusterResultStatus.ReplicasExhausted, backendReplicaResults, null, request);
            ClusterResultsAnalyzer.FindResultSource(clusterResult).Should().Be(ResultReason.Backend);
        }

        [TestCase(502, 502, ResultReason.Backend)]
        [TestCase(500, 502, ResultReason.Backend)]
        [TestCase(502, 500, ResultReason.Backend)]
        [TestCase(500, 500, ResultReason.InternalServerError)]
        [TestCase(500, 450, ResultReason.Complex)]
        public void Should_choose_correct_verdict_for_many_replicas_with_exhausted_status(ResponseCode responseCode1, ResponseCode responseCode2, ResultReason result)
        {
            var replicaResults = new List<ReplicaResult>
            {
                CreateRejectReplicaResult(responseCode1, cleanHeaders),
                CreateRejectReplicaResult(responseCode2, cleanHeaders)
            };

            var clusterResult = new ClusterResult(ClusterResultStatus.ReplicasExhausted, replicaResults, null, request);
            ClusterResultsAnalyzer.FindResultSource(clusterResult).Should().Be(result);
        }

        private ReplicaResult CreateRejectReplicaResult(ResponseCode code, Headers headers)
        {
            return new ReplicaResult(
                anyUri,
                new Response(
                    code,
                    null,
                    headers
                ),
                ResponseVerdict.Reject,
                TimeSpan.Zero
            );
        }
    }
}
