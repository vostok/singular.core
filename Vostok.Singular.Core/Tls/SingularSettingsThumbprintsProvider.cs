using System.Collections.Generic;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.Tls
{
    internal class SingularSettingsThumbprintsProvider : IThumbprintsProvider
    {
        private readonly SingularSettings settings;

        public SingularSettingsThumbprintsProvider(SingularSettings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<string> GetBlacklist() => settings.TlsClient.CertificateThumbprintsBlacklist;
        public IEnumerable<string> GetWhitelist() => settings.TlsClient.CertificateThumbprintsWhitelist;
    }
}