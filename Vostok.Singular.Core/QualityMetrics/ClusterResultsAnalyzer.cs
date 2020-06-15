using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal static class ClusterResultsAnalyzer
    {
        private static readonly Dictionary<ResponseCode, ResultReason> ReplicaExhaustedReasons = new Dictionary<ResponseCode, ResultReason>
        {
            {ResponseCode.Unknown, ResultReason.Unknown},
            {ResponseCode.RequestTimeout, ResultReason.RequestTimeout},
            {ResponseCode.ConnectFailure, ResultReason.ConnectFailure},
            {ResponseCode.ReceiveFailure, ResultReason.ReceiveFailure},
            {ResponseCode.SendFailure, ResultReason.SendFailure},
            {ResponseCode.InternalServerError, ResultReason.InternalServerError},
            {ResponseCode.ServiceUnavailable, ResultReason.ServiceUnavailable},
            {ResponseCode.ProxyTimeout, ResultReason.ProxyTimeout},
            {ResponseCode.UnknownFailure, ResultReason.UnknownFailure},
            {ResponseCode.StreamReuseFailure, ResultReason.StreamReuseFailure},
            {ResponseCode.StreamInputFailure, ResultReason.StreamInputFailure}
        };

        public static ResultReason FindResultSource(ClusterResult result) =>
            FindResultSource(new BriefClusterResult(result));

        public static ResultReason FindResultSource(BriefClusterResult result)
        {
            switch (result.Status)
            {
                case ClusterResultStatus.TimeExpired:
                    return ResultReason.TimeExpired;
                case ClusterResultStatus.ReplicasNotFound:
                    return ResultReason.SingularReplicasNotFound;
                case ClusterResultStatus.UnexpectedException:
                    return ResultReason.UnexpectedException;
                case ClusterResultStatus.ReplicasExhausted:
                    return FindResultReason(result.ReplicaResults);
                default:
                    return ResultReason.Backend;
            }
        }

        private static ResultReason FindResultReason(IList<BriefReplicaResult> replicaResults)
        {
            var severalDifferentReasons = false;
            ResultReason? lastReason = null;
            foreach (var replicaResult in replicaResults.Select(FindResultReason))
            {
                if (replicaResult == ResultReason.Backend)
                    return ResultReason.Backend;
                if (lastReason == null || lastReason == replicaResult)
                    lastReason = replicaResult;
                else
                    severalDifferentReasons = true;
            }

            if (severalDifferentReasons)
                return ResultReason.Complex;
            if (lastReason == null)
                return ResultReason.Backend;
            return (ResultReason)lastReason;
        }

        private static ResultReason FindResultReason(BriefReplicaResult replicaResult)
        {
            if (replicaResult.Code == ResponseCode.BadGateway || replicaResult.HasBackendHeader)
                return ResultReason.Backend;
            if (ReplicaExhaustedReasons.TryGetValue(replicaResult.Code, out var resultReason))
                return resultReason;
            if (replicaResult.Code == ResponseCode.TooManyRequests && replicaResult.HasIsSingularThrottlingTriggerHeader)
                return ResultReason.SingularThrottling;

            return ResultReason.Backend;
        }
    }
}