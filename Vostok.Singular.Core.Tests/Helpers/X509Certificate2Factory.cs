#if NETCOREAPP2_0_OR_GREATER
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tests.Helpers
{
    internal static class X509Certificate2Factory
    {
        public static X509Certificate2 Create(string commonName)
        {
            return Create(commonName, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));
        }

        public static X509Certificate2 Create(string commonName, DateTimeOffset notBefore, DateTimeOffset notAfter)
        {
            var req = GetCertificateRequest(commonName);
            // NOTE (tsup, 22-08-2022): Dirty hacks for signing certificates on the fly
            req.CertificateExtensions.Add(new X509BasicConstraintsExtension(new AsnEncodedData(new byte[] {0x30, 0x06, 0x01, 0x01, 0xFF, 0x02, 0x01, 0x01}), false));
            return req.CreateSelfSigned(notBefore, notAfter);
        }

        public static X509Certificate2 CreateSigned(string commonName, DateTimeOffset notBefore, DateTimeOffset notAfter, X509Certificate2 signer)
        {
            var req = GetCertificateRequest(commonName);
            return req.Create(signer, notBefore, notAfter, new byte[] {1, 2, 3, 4, 5});
        }

        public static X509Certificate2 CreateSigned(string commonName, X509Certificate2 signer)
        {
            return CreateSigned(commonName, DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5), signer);
        }

        public static X509Certificate2 CreateDefault()
        {
            return Create("Singular");
        }

        public static X509Certificate2 CreateDefaultExpired()
        {
            return Create("Singular", DateTimeOffset.Now - TimeSpan.FromDays(365), DateTimeOffset.Now - TimeSpan.FromDays(364));
        }

        public static X509Chain CreateChainFromCertificates(X509Certificate2 certificate, params X509Certificate2[] extra)
        {
            var chain = new X509Chain();

            foreach (var parentCertificate in extra)
                chain.ChainPolicy.ExtraStore.Add(parentCertificate);

            chain.Build(certificate);
            return chain;
        }

        private static CertificateRequest GetCertificateRequest(string commonName)
        {
            var ecdsa = ECDsa.Create();
            return new CertificateRequest($"cn={commonName}", ecdsa!, HashAlgorithmName.SHA256);
        }
    }
}
#endif