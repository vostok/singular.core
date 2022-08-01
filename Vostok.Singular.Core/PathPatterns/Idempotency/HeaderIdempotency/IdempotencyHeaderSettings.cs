using System;

namespace Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency
{
    [Serializable]
    internal class IdempotencyHeaderSettings
    {
        public bool OverrideHeader = false;
    }
}