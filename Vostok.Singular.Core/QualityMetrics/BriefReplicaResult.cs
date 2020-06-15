using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal class BriefReplicaResult
    {
        public ResponseCode Code;
        public bool HasIsSingularThrottlingTriggerHeader;
        public bool HasBackendHeader;

        public BriefReplicaResult(ResponseCode code, bool hasIsSingularThrottlingTriggerHeader, bool hasBackendHeader)
        {
            Code = code;
            HasIsSingularThrottlingTriggerHeader = hasIsSingularThrottlingTriggerHeader;
            HasBackendHeader = hasBackendHeader;
        }

        public BriefReplicaResult(ReplicaResult vostokReplicaResult)
        {
            Code = vostokReplicaResult.Response.Code;
            HasBackendHeader = vostokReplicaResult.Response.Headers[SingularHeaders.Backend] != null;
            HasIsSingularThrottlingTriggerHeader = vostokReplicaResult.Response.Headers[SingularHeaders.IsSingularThrottlingTrigger] != null;
        }
    }
}