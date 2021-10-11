using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Singular.Core.Configuration
{
    internal class VersionedSettings
    {
        public bool IsApiVersion { get; set; }
        public long Version { get; set; }
        public ISettingsNode Settings { get; set; }
    }
}