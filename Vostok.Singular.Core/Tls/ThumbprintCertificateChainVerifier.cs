using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    // https://www.openssl.org/docs/man1.1.1/man1/x509.html (fingerprints are unique)
    internal class ThumbprintCertificateChainVerifier : ICertificateChainVerifier
    {
        private readonly IThumbprintVerificationSettingsProvider verificationSettingsProvider;

        public ThumbprintCertificateChainVerifier(IThumbprintVerificationSettingsProvider verificationSettingsProvider)
        {
            this.verificationSettingsProvider = verificationSettingsProvider;
        }

        public bool VerifyChain(X509Chain chain)
        {
            var whitelist = verificationSettingsProvider.GetWhitelist();
            var blacklist = verificationSettingsProvider.GetBlacklist();

            return
                (whitelist.Count == 0 || GetCertificates(chain.ChainElements).Any(x => IsInListOfCertificates(x, whitelist))) &&
                !GetCertificates(chain.ChainElements).Any(x => IsInListOfCertificates(x, blacklist));
        }

        private static bool IsInListOfCertificates(X509Certificate2 certificate, IEnumerable<string> thumbprints)
        {
            return thumbprints.Any(x => ThumbprintsEqual(certificate, x));
        }

        // Thumbprints are not case-sensitive and must be computed using SHA1 algorithm
        // (https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509certificate2.thumbprint#remarks)
        private static bool ThumbprintsEqual(X509Certificate2 certificate, string thumbprint)
        {
            return certificate.Thumbprint!.Equals(thumbprint, StringComparison.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<X509Certificate2> GetCertificates(X509ChainElementCollection chainElements)
        {
            foreach (var chainElement in chainElements)
                yield return chainElement.Certificate;
        }
    }
}