using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal interface ICertificateChainVerifier
    {
        bool VerifyChain(X509Chain chain);
    }
}