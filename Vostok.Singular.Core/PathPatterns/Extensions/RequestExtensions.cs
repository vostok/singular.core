using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.PathPatterns.Extensions
{
    internal static class RequestExtensions
    {
        internal static Request WithNotIdempotentHeader(this Request request) =>
            request.WithHeader(SingularHeaders.Idempotent, false);

        internal static Request WithIdempotentHeader(this Request request) =>
            request.WithHeader(SingularHeaders.Idempotent, true);

        internal static string GetIdempotencyHeader(this Request request) =>
            request.Headers?[SingularHeaders.Idempotent];
    }
}