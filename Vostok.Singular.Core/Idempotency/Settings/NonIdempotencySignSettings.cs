using System;

namespace Vostok.Singular.Core.Idempotency.Settings
{
    [Serializable]
    internal class NonIdempotencySignSettings
    {
        public string Method;

        public string PathPattern;
    }
}