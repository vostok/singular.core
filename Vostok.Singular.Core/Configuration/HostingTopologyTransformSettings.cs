using System;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HostingTopologyTransformSettings
    {
        public bool UseHostingTopologyTransform = true;
        public double? AcceptableAliveBeaconsRatio;
        public double? MinCommonReplicasRatio;
    }
}