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

        public IList<string> GetBlacklist() => settings.TlsClient.CertificateThumbprintsBlacklist;
        public IList<string> GetWhitelist() => settings.TlsClient.CertificateThumbprintsWhitelist;
    }
}