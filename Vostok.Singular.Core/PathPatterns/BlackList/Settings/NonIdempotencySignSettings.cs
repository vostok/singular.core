using System;

#pragma warning disable 649

namespace Vostok.Singular.Core.PathPatterns.BlackList.Settings
{
    [Serializable]
    public class NonIdempotencySignSettings
    {
        public string Method;

        public string PathPattern;
    }
}