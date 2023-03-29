using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HeadersTransformationSettings
    {
        public Dictionary<string, string> Rewrites { get; set; } = new Dictionary<string, string>();
    }
}