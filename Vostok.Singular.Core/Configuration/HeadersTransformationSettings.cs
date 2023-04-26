using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HeadersTransformationSettings
    {
        public bool ProxyNginxMarkerHeader { get; set; } = false;
        public Dictionary<string, string> Rewrites { get; set; } = new Dictionary<string, string>();
    }
}