using System;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HttpHeadersTransformationSettings : HeadersTransformationSettings
    {
        public bool RewriteHostHeaderWithTargetHostValue = false;
    }
}