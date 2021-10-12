using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsUpdaterResult
    {
        public bool Changed { get; }
        public long Version { get; }

        public SettingsVersionType VersionType { get; }

        public ISettingsNode Settings { get; }

        public SettingsUpdaterResult(bool changed, long version, SettingsVersionType versionType, ISettingsNode settings)
        {
            Changed = changed;
            Version = version;
            VersionType = versionType;
            Settings = settings;
        }
    }
}