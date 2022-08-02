using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls
{
    internal interface ITlsHandshakeValidator
    {
        bool Verify(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors);
    }
}