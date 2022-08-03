using System.Collections.Generic;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.Tls
{
    internal class SingularSettingsThumbprintVerificationSettingsProvider : IThumbprintVerificationSettingsProvider
    {
        private readonly SingularSettings settings;

        public SingularSettingsThumbprintVerificationSettingsProvider(SingularSettings settings)
        {
            this.settings = settings;
        }

        public bool AllowAnyThumbprintExceptBlacklisted => settings.TlsClient.AllowAnyThumbprintExceptBlacklisted;
        public IEnumerable<string> GetBlacklist() => settings.TlsClient.CertificateThumbprintsBlacklist;
        public IEnumerable<string> GetWhitelist() => settings.TlsClient.CertificateThumbprintsWhitelist;
    }
}