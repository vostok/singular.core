using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal interface ITrustedCertificateVerifier
    {
        bool IsInWhitelist(X509Certificate2 certificate);
        bool IsInBlacklist(X509Certificate2 certificate);
    }
}