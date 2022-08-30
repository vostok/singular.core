#if NETCOREAPP2_0_OR_GREATER

using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Singular.Core.Tests.Helpers;
using Vostok.Singular.Core.Tls;

namespace Vostok.Singular.Core.Tests.Tls
{
    [TestFixture]
    internal class ThumbprintCertificateChainVerifier_Tests
    {
        private IThumbprintVerificationSettingsProvider settingsProvider;
        private ThumbprintCertificateChainAuthorityVerifier certificateChainAuthorityVerifier;

        [SetUp]
        public void SetUp()
        {
            settingsProvider = Substitute.For<IThumbprintVerificationSettingsProvider>();
            settingsProvider.GetBlacklist().ReturnsForAnyArgs(new List<string>());
            settingsProvider.GetWhitelist().ReturnsForAnyArgs(new List<string>());
            certificateChainAuthorityVerifier = new ThumbprintCertificateChainAuthorityVerifier(settingsProvider);
        }

        [Test]
        public void Should_allow_any_chain_by_default()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);
            certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_chain_with_blacklisted_element()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);

            foreach (var element in new[] {root, leaf})
            {
                settingsProvider.GetBlacklist().ReturnsForAnyArgs(new List<string> {element.Thumbprint});
                certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeFalse();
            }
        }

        [Test]
        public void Should_allow_chain_without_blacklisted_elements_when_whitelist_absent()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);

            settingsProvider.GetBlacklist().ReturnsForAnyArgs(new List<string> {"BLA"});
            certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_chain_without_whitelisted_element_when_whitelist_present()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);

            settingsProvider.GetWhitelist().ReturnsForAnyArgs(new List<string> {"BLA"});
            certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeFalse();
        }

        [Test]
        public void Should_allow_chain_with_any_whitelisted_element_when_whitelist_present()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);

            foreach (var element in new[] {root, leaf})
            {
                settingsProvider.GetWhitelist().ReturnsForAnyArgs(new List<string> {element.Thumbprint});
                certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeTrue();
            }
        }

        [Test]
        public void Should_not_allow_chain_with_whitelisted_and_blacklisted_elements()
        {
            var root = X509Certificate2Factory.CreateDefault();
            var leaf = X509Certificate2Factory.CreateSigned("child", root);
            var chain = X509Certificate2Factory.CreateChainFromCertificates(leaf, root);

            foreach (var first in new[] {root, leaf})
            {
                foreach (var second in new[] {root, leaf})
                {
                    settingsProvider.GetWhitelist().ReturnsForAnyArgs(new List<string> {first.Thumbprint});
                    settingsProvider.GetBlacklist().ReturnsForAnyArgs(new List<string> {second.Thumbprint});
                    certificateChainAuthorityVerifier.Verify(leaf, chain).Should().BeFalse();
                }
            }
        }

        // NOTE (tsup, 22-08-2022): This is actually a unit-test of dotnet API: chain creation does not allow specifying unlinked elements
        // therefore, in this example unlinked chain contains actual certificate, but does not contain trusted certificate
        [Test]
        public void Should_not_allow_invalid_trusted_chains()
        {
            var trustedCertificate = X509Certificate2Factory.CreateDefault();
            var actualCertificate = X509Certificate2Factory.CreateDefault();
            var unlinkedChain = X509Certificate2Factory.CreateChainFromCertificates(actualCertificate, trustedCertificate);

            settingsProvider.GetWhitelist().ReturnsForAnyArgs(new List<string> {trustedCertificate.Thumbprint});
            certificateChainAuthorityVerifier.Verify(actualCertificate, unlinkedChain).Should().BeFalse();
        }
    }
}

#endif