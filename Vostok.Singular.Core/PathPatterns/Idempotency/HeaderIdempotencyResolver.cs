namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal static class HeaderIdempotencyResolver
    {
        public static bool? IsIdempotentByHeader(string headerValue)
        {
            if (!bool.TryParse(headerValue, out var isIdempotent))
                return null;
            
            return isIdempotent;
        }
    }
}