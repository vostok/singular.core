using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal interface ICertificateChainAuthorityVerifier
    {
        bool Verify(X509Certificate certificate, X509Chain chain);
    }
}