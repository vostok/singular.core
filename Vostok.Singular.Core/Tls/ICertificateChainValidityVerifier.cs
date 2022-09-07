using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal interface ICertificateChainValidityVerifier
    {
        bool Verify(X509Certificate certificate, X509Chain chain, out X509ChainStatus[] statuses);
    }
}