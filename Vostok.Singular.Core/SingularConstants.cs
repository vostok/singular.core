using Vostok.Clusterclient.Core.Model;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core
{
    internal static class SingularConstants
    {
        public const string ProjectName = "Infrastructure";
        public const string ServiceName = "Singular";
        public const string CCTopologyName = "topology/singular";
        public const string DefaultZone = "default";
        public const string DefaultCluster = "global";

        public const string EnvironmentNamePath = "singular/environment/DefaultEnvironmentName";
        public const string CloudEnvironment = "Cloud";
        public const string ProdEnvironment = "Production";
        public const string SingularSettingsFileName = "singular.config.json";

        public const string CCTlsSettingsName = "singular/tls.json";

        public class DistributedProperties
        {
            public const string ForcedEnvironment = "forced.sd.environment";
        }
    }
}