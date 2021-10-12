namespace Vostok.Singular.Core.Configuration
{
    internal class VersionedSettings<TSettings>
    {
        public SettingsVersionType VersionType { get; set; }
        public long Version { get; set; }
        public TSettings Settings { get; set; }

        public VersionedSettings(SettingsVersionType versionType, long version, TSettings settings)
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