using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal class BriefReplicaResult
    {
        public BriefReplicaResult(ResponseCode code, bool isSingularInternalQuotasThrottled, bool isBackendResponse)
        {
            Code = code;
            IsSingularInternalQuotasThrottled = isSingularInternalQuotasThrottled;
            IsBackendResponse = isBackendResponse;
        }

        public BriefReplicaResult(ReplicaResult replicaResult)
        {
            var response = replicaResult.Response;
            Code = response.Code;
            IsBackendResponse = response.Headers[SingularHeaders.Backend] != null;
            IsSingularInternalQuotasThrottled = response.Headers[SingularHeaders.IsSingularInternalQuotasThrottling] != null;
        }

        public ResponseCode Code { get; }
        public bool IsSingularInternalQuotasThrottled { get; }
        public bool IsBackendResponse { get; }
    }
}