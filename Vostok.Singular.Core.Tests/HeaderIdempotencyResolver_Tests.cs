using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Singular.Core.PathPatterns;
using Vostok.Singular.Core.PathPatterns.Extensions;
using Vostok.Singular.Core.PathPatterns.Idempotency.HeaderIdempotency;

namespace Vostok.Singular.Core.Tests
{
    public class HeaderIdempotencyResolver_Tests
    {
        private IIdempotencySettingsProvider<IdempotencyHeaderSettings> settingsProvider;
        private HeaderIdempotencyResolver headerIdempotencyResolver;
        private Request request;

        [SetUp]
        public void Setup()
        {
            request = Request.Get("test");
            
            settingsProvider = Substitute.For<IIdempotencySettingsProvider<IdempotencyHeaderSettings>>();
            headerIdempotencyResolver = new HeaderIdempotencyResolver(settingsProvider);
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_null_if_settings_off()
        {
            SetupSettings();

            var result = await headerIdempotencyResolver.IsIdempotentAsync(request.GetIdempotencyHeader());

            result.Should().BeNull();
        }

        public async Task IsIdempotentAsync_should_return_null_if_settings_off_and_request_has_header()
        {
            SetupSettings();
            
            var result = await headerIdempotencyResolver.IsIdempotentAsync(
                request.WithHeader(SingularHeaders.Idempotent, true)
                    .GetIdempotencyHeader());

            result.Should().BeNull();
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_idempotency_header_value()
        {
            SetupSettings(true);
            
            var result = await headerIdempotencyResolver.IsIdempotentAsync(
                request.WithHeader(SingularHeaders.Idempotent, true)
                    .GetIdempotencyHeader());

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task IsIdempotentAsync_should_return_null_if_no_header()
        {
            SetupSettings(true);
            
            var result = await headerIdempotencyResolver.IsIdempotentAsync(
                request.GetIdempotencyHeader());

            result.Should().BeNull();
        }
        
        [Test]
        public async Task IsIdempotentAsync_should_return_null_if_header_is_not_bool()
        {
            SetupSettings(true);
            
            var result = await headerIdempotencyResolver.IsIdempotentAsync(
                request.WithHeader(SingularHeaders.Idempotent, "IggyPop")
                    .GetIdempotencyHeader());

            result.Should().BeNull();
        }

        private void SetupSettings(bool overrideHeader = false)
        {
            settingsProvider.GetAsync()
                .ReturnsForAnyArgs(new IdempotencyHeaderSettings
                {
                    OverrideHeader = overrideHeader
                });
        }
    }
}