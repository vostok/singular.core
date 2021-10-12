using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Singular.Core.Configuration
{
    internal class VersionedSettings
    {
        public SettingsVersionType VersionType { get; set; }
        public long Version { get; set; }
        public ISettingsNode Settings { get; set; }

        public VersionedSettings(SettingsVersionType versionType, long version, ISettingsNode settings)
        {
            VersionType = versionType;
            Version = version;
            Settings = settings;
        }

        public VersionedSettings() 
        {
            // For deserialization
        }
    }

    internal enum SettingsVersionType
    {
        ClusterConfig = 0,
        PublicationApi = 1
    }
}