using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using NSubstitute;
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
            var verifier = Substitute.For<ICertificateChainVerifier>();
            verifier.VerifyChain(null).ReturnsForAnyArgs(true);
            handshakeValidator = new SingularHandshakeValidator(verifier, new SilentLog());
        }

        [Test]
        public void Should_return_true_if_OS_trusts()
        {
            handshakeValidator.Verify(null, null, null, SslPolicyErrors.None).Should().BeTrue();
        }

        [TestCase(SslPolicyErrors.RemoteCertificateNameMismatch, Description = "Certificate name does not match FQDN")]
        [TestCase(SslPolicyErrors.RemoteCertificateNotAvailable, Description = "Certificate is not present or unavailable")]
        public void Should_not_allow_policy_errors_except_chain_errors(SslPolicyErrors error)
        {
            handshakeValidator.Verify(null, null, null, error).Should().BeFalse();
        }

#if NETCOREAPP2_0_OR_GREATER
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
            var verifier = Substitute.For<ICertificateChainVerifier>();
            verifier.VerifyChain(null).ReturnsForAnyArgs(false);
            handshakeValidator = new SingularHandshakeValidator(verifier, new SilentLog());
            handshakeValidator.Verify(null, generated, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
        }

        [Test]
        public void Should_not_allow_expired_certificates()
        {
            var generated = X509Certificate2Factory.CreateDefaultExpired();
            var chain = X509Certificate2Factory.CreateChainFromCertificates(generated);
            handshakeValidator.Verify(null, generated, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
        }

        [Test]
        public void Should_not_allow_expired_certificates_in_chain()
        {
            var root = X509Certificate2Factory.CreateDefaultExpired();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);
            handshakeValidator.Verify(null, leaf, chain, SslPolicyErrors.RemoteCertificateChainErrors).Should().BeFalse();
        }
#endif
    }
}