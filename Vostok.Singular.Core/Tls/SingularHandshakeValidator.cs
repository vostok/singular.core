using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.Tls
{
    internal class SingularHandshakeValidator : ITlsHandshakeValidator
    {
        private readonly ITrustedCertificateVerifier certificateVerifier;
        private readonly SingularSettings singularSettings;
        private readonly ILog log;

        public SingularHandshakeValidator(ITrustedCertificateVerifier certificateVerifier, SingularSettings singularSettings, ILog log)
        {
            this.certificateVerifier = certificateVerifier;
            this.singularSettings = singularSettings;
            this.log = log;
        }

        public bool Verify(object _, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Certificate (and the whole chain) is trusted by OS. We can safely proceed.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // There are some inappropriate problems with the chain. We can't safely verify it.
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch) ||
                sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable) ||
                chain == null || chain.ChainElements.Count == 0)
                return false;

            /*
             * At this point in time we know that the chain of certificates has some problems (Might be appropriate or not).
             * If there is any problem except of 'unknown certificate', we can't trust this chain.
             * (What if an attacker sends an invalid sequence of certificates, i.e. his self-signed certificate along with certificate of Kontur CA?)
             * Answer: this scenario is covered by .NET, we can't see an invalid sequence of certificates here.
             *      It means that we only have to check their individual validity and that we trust at least one certificate in the chain.
             *          (https://github.com/dotnet/runtime/issues/49615)
             *      That said, we have to check whether we have at least one certificate from whitelist and none of the certificates are from the blacklist.
             */

            if (!VerifyChain(chain))
                return false;

            /*
             * Now we only have to make sure that there are no problems with the chain apart from 'unknown' source.
             * Custom-made certificates have Unknown Revocation Status (due to absence of CRL)
             * However, we don't want to skip revocation check when CRL is present.
             * Therefore, we set revocation mode to online, BUT we ignore cases when the result is unknown.
             * (This way we will skip revocation check in case of its absence OR in case of network problems, which is fine in most cases).
             */

            // note (tsup, 14.07.2022): Seemingly, we can't reuse it thread-safely
            using var verifier = CreateVerifier();

            foreach (var chainElement in chain.ChainElements)
                verifier.ChainPolicy.ExtraStore.Add(chainElement.Certificate);

            var verificationResult = verifier.Build((certificate as X509Certificate2)!);

            if (!verificationResult)
                log.Info($"{verifier.ChainStatus}");

            return verificationResult;
        }

        private X509Chain CreateVerifier()
        {
            return new X509Chain
            {
                // todo: we should probably allow to tune some settings there
                ChainPolicy =
                {
                    VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority |
                                        X509VerificationFlags.IgnoreEndRevocationUnknown |
                                        X509VerificationFlags.IgnoreRootRevocationUnknown |
                                        X509VerificationFlags.IgnoreCertificateAuthorityRevocationUnknown |
                                        X509VerificationFlags.IgnoreCtlSignerRevocationUnknown,
                    RevocationMode = X509RevocationMode.Online,
                    RevocationFlag = X509RevocationFlag.ExcludeRoot
                }
            };
        }

        private bool VerifyChain(X509Chain chain)
        {
            return
                (singularSettings.TlsClient.AllowAnyThumbprintExceptBlacklisted || GetCertificates(chain.ChainElements).Any(certificateVerifier.IsInWhitelist)) &&
                !GetCertificates(chain.ChainElements).Any(certificateVerifier.IsInBlacklist);
        }

        private static IEnumerable<X509Certificate2> GetCertificates(X509ChainElementCollection chainElements)
        {
            foreach (var chainElement in chainElements)
                yield return chainElement.Certificate;
        }
    }
}