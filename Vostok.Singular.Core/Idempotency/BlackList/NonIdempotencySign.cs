namespace Vostok.Singular.Core.Idempotency.BlackList
{
    internal class NonIdempotencySign
    {
        public string Method;

        public Wildcard PathPattern;

        public NonIdempotencySign(string method, string pathPattern)
        {
            Method = method;
            PathPattern = pathPattern == null ? null : new Wildcard(pathPattern);
        }
    }
}