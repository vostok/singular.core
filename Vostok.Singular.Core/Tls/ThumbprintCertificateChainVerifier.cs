using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal class ThumbprintCertificateChainVerifier : ICertificateChainVerifier
    {
        private readonly IThumbprintVerificationSettingsProvider verificationSettingsProvider;

        public ThumbprintCertificateChainVerifier(IThumbprintVerificationSettingsProvider verificationSettingsProvider)
        {
            this.verificationSettingsProvider = verificationSettingsProvider;
        }

        public bool VerifyChain(X509Chain chain)
        {
            return
                (verificationSettingsProvider.AllowAnyThumbprintExceptBlacklisted || GetCertificates(chain.ChainElements).Any(IsInWhitelist)) &&
                !GetCertificates(chain.ChainElements).Any(IsInBlacklist);
        }

        private bool IsInWhitelist(X509Certificate2 certificate) => IsInListOfThumbprints(certificate, verificationSettingsProvider.GetWhitelist());

        private bool IsInBlacklist(X509Certificate2 certificate) => IsInListOfThumbprints(certificate, verificationSettingsProvider.GetBlacklist());

        // https://www.openssl.org/docs/man1.1.1/man1/x509.html (fingerprints are unique)
        private static bool IsInListOfThumbprints(X509Certificate2 certificate, IEnumerable<string> thumbprints)
        {
            return thumbprints.Any(x => certificate.Thumbprint!.Equals(x, StringComparison.InvariantCultureIgnoreCase));
        }

        private static IEnumerable<X509Certificate2> GetCertificates(X509ChainElementCollection chainElements)
        {
            foreach (var chainElement in chainElements)
                yield return chainElement.Certificate;
        }
    }
}