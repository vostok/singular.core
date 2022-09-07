using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Vostok.Logging.Abstractions;

namespace Vostok.Singular.Core.Tls
{
    internal class SingularHandshakeValidator : ITlsHandshakeValidator
    {
        private readonly ICertificateChainAuthorityVerifier certificateChainAuthorityVerifier;
        private readonly ICertificateChainValidityVerifier certificateChainValidityVerifier;
        private readonly ILog log;

        public SingularHandshakeValidator(
            ICertificateChainAuthorityVerifier certificateChainAuthorityVerifier,
            ICertificateChainValidityVerifier certificateChainValidityVerifier,
            ILog log)
        {
            this.certificateChainAuthorityVerifier = certificateChainAuthorityVerifier;
            this.certificateChainValidityVerifier = certificateChainValidityVerifier;
            this.log = log;
        }

        public bool Verify(object _, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Certificate (and the whole chain) is trusted by OS. We can safely proceed.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Provided chain is not available. We can't safely verify it.
            if (chain == null || chain.ChainElements.Count == 0 ||
                sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                LogUnavailableChain();
                return false;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                LogCertificateNameMismatch();
                return false;
            }

            /*
             * At this point in time we know that the chain of certificates has some problems (Might be appropriate or not).
             * If there is any problem except of 'unknown certificate', we can't trust this chain.
             *
             * Security question: What if an attacker sends an invalid sequence of certificates,
             *      i.e. his self-signed certificate along with a certificate of trusted CA? (In this example there is no link between the certificates.)
             * Answer: this scenario is covered by .NET, we can't see an invalid sequence of certificates here.
             *      It means that we only have to check their individual validity and that we trust at least one certificate in the chain.
             *          (https://github.com/dotnet/runtime/issues/49615)
             *      That said, we have to check whether we have at least one certificate from whitelist and none of the certificates are from the blacklist.
             *      (See ThumbprintCertificateChainAuthorityVerifier for implementation of this mechanic)
             */

            if (!certificateChainAuthorityVerifier.Verify(certificate, chain))
            {
                LogUntrustedChain();
                return false;
            }

            /*
             * Now we only have to make sure that there are no problems with the chain apart from 'unknown' source (which we already verified).
             * Custom-made certificates have Unknown Revocation Status (due to absence of CRL)
             * However, we don't want to skip revocation check when CRL is present.
             * Therefore, we set revocation mode to online, BUT we ignore cases when the result is unknown.
             * (This way we will skip revocation check in case of its absence OR in case of network problems, which is fine in most cases).
             * (See SimpleChainValidityVerifier for implementation of this mechanic)
             */

            if (!certificateChainValidityVerifier.Verify(certificate, chain, out var statuses))
            {
                LogFailedVerificationResult(statuses);
                return false;
            }

            return true;
        }

        #region Logging

        private void LogUnavailableChain()
        {
            log.Warn("Certificate chain is unavailable and can't be verified.");
        }

        private void LogCertificateNameMismatch()
        {
            log.Warn("Certificate name does not match the request name.");
        }

        private void LogUntrustedChain()
        {
            log.Warn("Certificate chain was rejected by the provided {HandshakeValidatorType}.", typeof(ITlsHandshakeValidator));
        }

        private void LogFailedVerificationResult(X509ChainStatus[] chainStatuses)
        {
            log.Warn($"Certificate chain was rejected by the {nameof(X509Chain)} verification. Statuses: {{Statuses}}", chainStatuses);
        }

        #endregion
    }
}