using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HeadersTransformationSettings
    {
        public string ProxyClientIpHeader { get; set; } = string.Empty;
        public bool ProxyOriginalUriHeader { get; set; } = false;
        public bool ProxyNginxMarkerHeader { get; set; } = false;
        public Dictionary<string, string> Rewrites { get; set; } = new Dictionary<string, string>();
    }
}