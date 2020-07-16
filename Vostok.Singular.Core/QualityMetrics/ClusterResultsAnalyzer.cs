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
            {ResponseCode.UnknownFailure, ResultReason.UnknownFailure},
            {ResponseCode.StreamReuseFailure, ResultReason.StreamReuseFailure},
            {ResponseCode.StreamInputFailure, ResultReason.StreamInputFailure}
        };

        public static ResultReason FindResultReason(ClusterResult result)
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

        private static ResultReason FindResultReason(IList<ReplicaResult> replicaResults)
        {
            ResultReason? lastReason = null;
            foreach (var replicaResult in replicaResults.Select(FindResultReason))
            {
                if (replicaResult == ResultReason.Backend)
                    return ResultReason.Backend;
                if (lastReason == null || lastReason == replicaResult)
                    lastReason = replicaResult;
                else
                    return ResultReason.Complex;
            }
            
            return lastReason ?? ResultReason.Backend;
        }

        private static ResultReason FindResultReason(ReplicaResult replicaResult)
        {
            var response = replicaResult.Response;
            var isBackendResponse = response.Headers[SingularHeaders.Backend] != null;
            var isSingularInternalQuotasThrottled = response.Headers[SingularHeaders.IsSingularInternalQuotasThrottling] != null;
            
            if (response.Code == ResponseCode.BadGateway)
                return ResultReason.Backend;
            if (ReplicaExhaustedReasons.TryGetValue(response.Code, out var resultReason))
                return resultReason;
            if (response.Code == ResponseCode.TooManyRequests && isSingularInternalQuotasThrottled)
                return ResultReason.SingularThrottling;

            return ResultReason.Backend;
        }
    }
}