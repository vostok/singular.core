using System;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HostHeaderTransformationSettings
    {
        public bool UseTargetHostValue { get; set; }
        public string CustomValue { get; set; }
    }
}