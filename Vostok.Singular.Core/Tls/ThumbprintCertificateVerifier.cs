using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal class ThumbprintCertificateVerifier : ITrustedCertificateVerifier
    {
        private readonly IThumbprintsProvider thumbprintsProvider;

        public ThumbprintCertificateVerifier(IThumbprintsProvider thumbprintsProvider)
        {
            this.thumbprintsProvider = thumbprintsProvider;
        }

        public bool IsInWhitelist(X509Certificate2 certificate) => IsInListOfThumbprints(certificate, thumbprintsProvider.GetWhitelist());

        public bool IsInBlacklist(X509Certificate2 certificate) => IsInListOfThumbprints(certificate, thumbprintsProvider.GetBlacklist());

        // https://www.openssl.org/docs/man1.1.1/man1/x509.html (fingerprints are unique)
        private static bool IsInListOfThumbprints(X509Certificate2 certificate, IEnumerable<string> thumbprints)
        {
            return thumbprints.Any(x => certificate.Thumbprint!.Equals(x, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}