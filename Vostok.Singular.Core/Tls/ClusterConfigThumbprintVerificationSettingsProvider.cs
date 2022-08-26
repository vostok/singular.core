using System;
using System.Collections.Generic;
using Vostok.ClusterConfig.Client;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.ClusterConfig;
using Vostok.Configuration.Sources.Json;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.Tls
{
    internal class ClusterConfigThumbprintVerificationSettingsProvider : IThumbprintVerificationSettingsProvider
    {
        private static readonly Lazy<ClusterConfigThumbprintVerificationSettingsProvider> DefaultInstance = new(CreateDefault);
        private readonly IConfigurationSource settingsSource;

        public ClusterConfigThumbprintVerificationSettingsProvider(IClusterConfigClient clusterConfigClient, ClusterConfigPath thumbprintsPath)
        {
            settingsSource = CreateSource(clusterConfigClient, thumbprintsPath);
        }

        public static ClusterConfigThumbprintVerificationSettingsProvider Default => DefaultInstance.Value;

        public IList<string> GetWhitelist()
        {
            return GetTlsSettings().CertificateThumbprintsWhitelist;
        }

        public IList<string> GetBlacklist()
        {
            return GetTlsSettings().CertificateThumbprintsBlacklist;
        }

        private SingularSettings.TlsClientSettings GetTlsSettings()
        {
            return ConfigurationProvider.Default.Get<SingularSettings.TlsClientSettings>(settingsSource);
        }

        private static IConfigurationSource CreateSource(IClusterConfigClient client, ClusterConfigPath path)
        {
            return new ClusterConfigSource(new ClusterConfigSourceSettings(client, path.ToString())
            {
                ValuesParser = (value, _) => JsonConfigurationParser.Parse(value)
            });
        }

        private static ClusterConfigThumbprintVerificationSettingsProvider CreateDefault()
        {
            return new ClusterConfigThumbprintVerificationSettingsProvider(
                ClusterConfigClient.Default,
                SingularConstants.CCTlsSettingsName
            );
        }
    }
}