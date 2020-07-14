using System;

namespace Vostok.Singular.Core.Idempotency.BlackList.Settings
{
    [Serializable]
    internal class NonIdempotencySignSettings
    {
        public string Method;

        public string PathPattern;
    }
}