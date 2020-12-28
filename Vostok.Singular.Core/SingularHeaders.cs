namespace Vostok.Singular.Core
{
    // todo (andrew, 28.12.2020): add response header indicating that response came from target service replica
    internal class SingularHeaders
    {
        public const string Environment = "X-Singular-Zone";
        public const string Service = "X-Singular-Service";
        public const string Timeout = "X-Singular-Timeout";
        public const string Backend = "X-Singular-Backend";
        public const string Replica = "X-Singular-Replica";
        public const string NginxMarker = "X-Singular-Nginx-Marker";
        public const string IsSingularInternalQuotasThrottling = "X-Singular-Throttling-Trigger";
        public const string XRealIP = "X-Real-IP";
        public const string XAccelBuffering = "X-Accel-Buffering";
        public const string XNginxDistributedContext = "X-Nginx-Distributed-Context";
    }
}