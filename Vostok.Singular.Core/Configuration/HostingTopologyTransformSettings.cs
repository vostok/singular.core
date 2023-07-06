using System;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class HostingTopologyTransformSettings
    {
        public bool UseHostingTopologyTransform = false;
        public double? AcceptableAliveBeaconsRatio;
        public double? MinCommonReplicasRatio;
    }
}