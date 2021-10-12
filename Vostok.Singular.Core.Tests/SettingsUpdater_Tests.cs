using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Printing;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns;

namespace Vostok.Singular.Core.Tests
{
    [TestFixture]
    public class SettingsUpdater_Tests
    {
        private const string Service = "srv1";
        private const string Environment = "env1";
        private SettingsUpdater settingsUpdater;
        private IClusterClient singularClient;

        [SetUp]
        public void TestSetup()
        {
            singularClient = Substitute.For<IClusterClient>();

            settingsUpdater = new SettingsUpdater(singularClient);
        }

        [Test]
        public async Task Updater_should_throw_exception_if_singular_code_is_not_200()
        {
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(ServerErrorResult()));

            Func<Task> assertion = async () =>
            {
                await settingsUpdater.UpdateAsync(Environment, Service);
            };

            await assertion.Should()
                .ThrowExactlyAsync<Exception>()
                .WithMessage("Failed to update idempotency settings from singular. Response code = InternalServerError.");
        }

        [Test]
        public async Task Updater_should_throw_exception_if_response_body_is_empty()
        {
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(OkResult("")));

            Func<Task> assertion = async () =>
            {
                await settingsUpdater.UpdateAsync(Environment, Service);
            };

            await assertion.Should()
                .ThrowExactlyAsync<Exception>()
                .WithMessage("Received an empty 200 response from server.");
        }

        [Test]
        public async Task Updater_should_throw_exception_if_no_current_version_is_provided_and_response_code_304()
        {
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(NotModifiedResult()));

            Func<Task> assertion = async () =>
            {
                await settingsUpdater.UpdateAsync(Environment, Service);
            };

            await assertion.Should()
                .ThrowExactlyAsync<Exception>()
                .WithMessage("Received unexpected 'NotModified' response from server although no current version was sent in request.");
        }

        [Test]
        public async Task Updater_should_throw_exception_if_settings_bind_to_null()
        {
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(OkResult("{}")));

            Func<Task> assertion = async () =>
            {
                await settingsUpdater.UpdateAsync(Environment, Service);
            };

            await assertion.Should()
                .ThrowExactlyAsync<Exception>()
                .WithMessage("Received unexpected empty settings. Content: {}");
        }

        [Test]
        public async Task Updater_should_return_non_changed_result_if_version_not_modified()
        {
            var previousUpdateResult = new SettingsUpdaterResult(false, 1, SettingsVersionType.ClusterConfig, null);
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(NotModifiedResult()));

            var actualResult = await settingsUpdater.UpdateAsync(Environment, Service, previousUpdateResult);

            actualResult.Should().NotBeNull();
            actualResult.Changed.Should().BeFalse();
            actualResult.Version.Should().Be(previousUpdateResult.Version);
            actualResult.VersionType.Should().Be(previousUpdateResult.VersionType);
            actualResult.Settings.Should().BeNull();
        }

        [Test]
        public async Task Updater_should_return_changed_result_if_version_modified()
        {
            var previousUpdateResult = new SettingsUpdaterResult(false, 1, SettingsVersionType.ClusterConfig, null);
            var settings = new VersionedSettings(SettingsVersionType.PublicationApi, 2 , new ObjectNode(""));
            singularClient.SendAsync(Arg.Any<Request>())
                .Returns(info => Task.FromResult(OkResult(ConfigurationPrinter.Print(settings, new PrintSettings(){Format = PrintFormat.JSON}))));

            var actualResult = await settingsUpdater.UpdateAsync(Environment, Service, previousUpdateResult);

            actualResult.Should().NotBeNull();
            actualResult.Changed.Should().BeTrue();
            actualResult.Version.Should().Be(settings.Version);
            actualResult.VersionType.Should().Be(settings.VersionType);
            actualResult.Settings.Should().NotBeNull();
        }

        private static ClusterResult ServerErrorResult() =>
            new ClusterResult(ClusterResultStatus.ReplicasExhausted, new List<ReplicaResult>(), new Response(ResponseCode.InternalServerError), Request.Get(""));
        private static ClusterResult OkResult(string content) =>
            new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), new Response(ResponseCode.Ok, new Content(Encoding.UTF8.GetBytes(content))), Request.Get(""));
        private static ClusterResult NotModifiedResult() =>
            new ClusterResult(ClusterResultStatus.Success, new List<ReplicaResult>(), new Response(ResponseCode.NotModified), Request.Get(""));
    }
}