using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsUpdaterResult
    {
        public bool Changed { get; }
        public long Version { get; }

        public bool IsApiVersion { get; }

        public ISettingsNode Settings { get; }

        public SettingsUpdaterResult(bool changed, long version, bool isApiVersion, ISettingsNode settings)
        {
            Changed = changed;
            Version = version;
            IsApiVersion = isApiVersion;
            Settings = settings;
        }
    }
}