namespace Vostok.Singular.Core
{
    internal static class SingularHeaders
    {
        public const string Environment = "X-Singular-Zone";
        public const string Service = "X-Singular-Service";
        public const string Timeout = "X-Singular-Timeout";
        public const string Backend = "X-Singular-Backend";
        public const string Replica = "X-Singular-Replica";
        public const string NginxMarker = "X-Singular-Nginx-Marker";
        public const string SingularThrottlingTrigger = "X-Singular-Throttling-Trigger";
        public const string ReplicaTagsFilter = "X-Singular-Replica-Tags-Filter";
        public const string XRealIP = "X-Real-IP";
        public const string XAccelBuffering = "X-Accel-Buffering";
        public const string XNginxDistributedContext = "X-Nginx-Distributed-Context";

        public static class ThrottlingTriggerReason
        {
            public const string ServerThrottlingQueueOverflow = "ServerThrottlingQueueOverflow";
            public const string ServerQuotaExhaustion = "ServerQuotaExhaustion";
            public const string UserQuotaExhaustion = "UserQuotaExhaustion";
            public const string AdaptiveClientThrottling = "AdaptiveClientThrottling";
        }
    }
}
