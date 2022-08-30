using System.Security.Cryptography.X509Certificates;

namespace Vostok.Singular.Core.Tls;

internal class SimpleChainValidityVerifier : ICertificateChainValidityVerifier
{
    public bool Verify(X509Certificate certificate, X509Chain chain, out X509ChainStatus[] statuses)
    {
        // note (tsup, 14.07.2022): Seemingly, we can't reuse it thread-safely
        using var verifier = CreateVerifier();

        foreach (var chainElement in chain.ChainElements)
            verifier.ChainPolicy.ExtraStore.Add(chainElement.Certificate);

        verifier.Build((certificate as X509Certificate2)!);

        var verificationResult = verifier.Build((certificate as X509Certificate2)!);
        statuses = verifier.ChainStatus;
        return verificationResult;
    }

    private static X509Chain CreateVerifier()
    {
        return new X509Chain
        {
            ChainPolicy =
            {
                VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority |
                                    X509VerificationFlags.IgnoreEndRevocationUnknown |
                                    X509VerificationFlags.IgnoreRootRevocationUnknown |
                                    X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown |
                                    X509VerificationFlags.IgnoreCtlSignerRevocationUnknown,
                RevocationMode = X509RevocationMode.Online,
                RevocationFlag = X509RevocationFlag.EntireChain
            }
        };
    }
}