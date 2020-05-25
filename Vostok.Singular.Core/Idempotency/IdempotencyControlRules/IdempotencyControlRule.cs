namespace Vostok.Singular.Core.Idempotency.IdempotencyControlRules
{
    internal class IdempotencyControlRule
    {
        public string Method;

        public Wildcard PathPattern;

        public bool IsIdempotent;
    }
}