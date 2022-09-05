using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Vostok.Logging.Abstractions;
using Vostok.Singular.Core.Tls;
#if NETCOREAPP2_0_OR_GREATER
using Vostok.Singular.Core.Tests.Helpers;
#endif

namespace Vostok.Singular.Core.Tests.Tls
{
    [TestFixture]
    internal class SingularHandshakeValidator_Tests
    {
        private SingularHandshakeValidator handshakeValidator;

        [SetUp]
        public void SetUp()
        {
            var authorityVerifier = Substitute.For<ICertificateChainAuthorityVerifier>();
            authorityVerifier.Verify(null, null).ReturnsForAnyArgs(true);
            var validityVerifier = Substitute.For<ICertificateChainValidityVerifier>();
            validityVerifier.Verify(null, null, out _).ReturnsForAnyArgs(true);
            handshakeValidator = new SingularHandshakeValidator(authorityVerifier, validityVerifier, new SilentLog());
        }

        [Test]
        public void Should_return_true_if_OS_trusts()
        {
            handshakeValidator.Verify(null, null, null, SslPolicyErrors.None).Should().BeTrue();
        }

#if NETCOREAPP2_0_OR_GREATER
        [TestCase(SslPolicyErrors.RemoteCertificateNameMismatch, Description = "Certificate name does not match FQDN")]
        [TestCase(SslPolicyErrors.RemoteCertificateNotAvailable, Description = "Certificate is not present or unavailable")]
        public void Should_not_allow_policy_errors_except_chain_errors(SslPolicyErrors error)
        {
            var generated = X509Certificate2Factory.CreateDefault();
            var chain = X509Certificate2Factory.CreateChainFromCertificates(generated);
            handshakeValidator.Verify(null, generated, chain, error).Should().BeFalse();
        }

        [Test]
        public void Should_allow_self_signed_certificates()
        {
            var generated = X509Certificate2Factory.CreateDefault();
            var chain = X509Certificate2Factory.CreateChainFromCertificates(generated);
            handshakeValidator.Verify(null, generated, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_untrusted_chains()
        {
            var generated = X509Certificate2Factory.CreateDefault();
            var chain = X509Certificate2Factory.CreateChainFromCertificates(generated);
            var verifier = Substitute.For<ICertificateChainAuthorityVerifier>();
            verifier.Verify(null, null).ReturnsForAnyArgs(false);
            var validityVerifier = Substitute.For<ICertificateChainValidityVerifier>();
            validityVerifier.Verify(null, null, out _).ReturnsForAnyArgs(true);
            handshakeValidator = new SingularHandshakeValidator(verifier, validityVerifier, new SilentLog());
            handshakeValidator.Verify(null, generated, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
            verifier.ReceivedWithAnyArgs(1).Verify(null, null);
        }

        [Test]
        public void Should_not_allow_expired_certificates()
        {
            var generated = X509Certificate2Factory.CreateDefaultExpired();
            var chain = X509Certificate2Factory.CreateChainFromCertificates(generated);
            var verifier = Substitute.For<ICertificateChainAuthorityVerifier>();
            verifier.Verify(null, null).ReturnsForAnyArgs(true);
            handshakeValidator = new SingularHandshakeValidator(verifier, new SimpleChainValidityVerifier(), new SilentLog());
            handshakeValidator.Verify(null, generated, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
        }

        [Test]
        public void Should_not_allow_expired_certificates_in_chain()
        {
            var root = X509Certificate2Factory.CreateDefaultExpired();
            var leaf = X509Certificate2Factory.CreateSignedExpired("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);
            var verifier = Substitute.For<ICertificateChainAuthorityVerifier>();
            verifier.Verify(null, null).ReturnsForAnyArgs(true);
            handshakeValidator = new SingularHandshakeValidator(verifier, new SimpleChainValidityVerifier(), new SilentLog());
            handshakeValidator.Verify(null, leaf, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
        }
#endif
    }
}