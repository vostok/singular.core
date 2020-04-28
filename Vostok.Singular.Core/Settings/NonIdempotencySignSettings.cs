using System;

namespace Vostok.Singular.Core.Settings
{
    [Serializable]
    internal class NonIdempotencySignSettings
    {
        public string Method;

        public string PathPattern;
    }
}