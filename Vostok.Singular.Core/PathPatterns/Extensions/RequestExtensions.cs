using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.PathPatterns.Extensions
{
    internal static class RequestExtensions
    {
        internal static string GetIdempotencyHeader(this Request request) =>
            request.Headers?[SingularHeaders.Idempotent];
    }
}