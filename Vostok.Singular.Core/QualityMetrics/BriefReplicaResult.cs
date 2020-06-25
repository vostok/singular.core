using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    // CR: комментарии как в BriefClusterResult.
    internal class BriefReplicaResult
    {
        public ResponseCode Code;

        // CR: IsSingularInternalQuotasThrottling?.
        public bool HasIsSingularThrottlingTriggerHeader;

        // CR: IsBackendResponse? 
        public bool HasBackendHeader;

        public BriefReplicaResult(ResponseCode code, bool hasIsSingularThrottlingTriggerHeader, bool hasBackendHeader)
        {
            Code = code;
            HasIsSingularThrottlingTriggerHeader = hasIsSingularThrottlingTriggerHeader;
            HasBackendHeader = hasBackendHeader;
        }

        public BriefReplicaResult(ReplicaResult vostokReplicaResult)
        {
            var response = vostokReplicaResult.Response;

            Code = response.Code;
            HasBackendHeader = response.Headers[SingularHeaders.Backend] != null;
            HasIsSingularThrottlingTriggerHeader = response.Headers[SingularHeaders.IsSingularThrottlingTrigger] != null;
        }
    }
}