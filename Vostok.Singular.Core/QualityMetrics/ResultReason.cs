namespace Vostok.Singular.Core.QualityMetrics
{
    internal enum ResultReason
    {
        Backend,
        TimeExpired,
        SingularReplicasNotFound,
        UnexpectedException,
        Unknown,
        RequestTimeout,
        ConnectFailure,
        ReceiveFailure,
        SendFailure,
        InternalServerError,
        ServiceUnavailable,
        ProxyTimeout,
        SingularThrottling,
        UnknownFailure,
        StreamReuseFailure,
        StreamInputFailure,
        Complex
    }
}