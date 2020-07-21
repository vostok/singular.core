using System;

namespace Vostok.Singular.Core.PathPatterns.BlackList.Settings
{
    [Serializable]
    internal class NonIdempotencySignSettings
    {
        public string Method;

        public string PathPattern;
    }
}