using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Configuration;
using Vostok.Configuration.Printing;
using Vostok.Singular.Core.Configuration;
using Vostok.Singular.Core.PathPatterns.Idempotency;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;
using Vostok.Singular.Core.PathPatterns.SettingsAlias;
using Vostok.Singular.Core.PathPatterns.Timeout;

namespace Vostok.Singular.Core.Tests
{
    internal class CacheProviders_Tests
    {
        [Test]
        public async Task IdempotencyIdentifierCache_should_detect_identity_correctly()
        {
            var settings = new SingularSettings
            {
                IdempotencySettings = new IdempotencySettings
                {
                    Rules = new List<IdempotencyRuleSetting>
                    {
                        new IdempotencyRuleSetting
                        {
                            Method = "*",
                            PathPattern = "*",
                            IsIdempotent = false
                        }
                    }
                }
            };
            
            var service = Guid.NewGuid().ToString();
            var client = CreateClientWithServiceSettings(settings, "not-default", service);
            var cache = IdempotencyIdentifierCache.Get(client, "not-default", service);
            var result = await cache.IsIdempotentAsync("", "");

            result.Should().BeFalse();
        }

        [Test]
        public async Task TimeoutSettingsProviderCache_should_detect_identity_correctly()
        {
            var settings = new SingularSettings
            {
                Defaults = new SingularSettings.DefaultsSettings
                {
                    TimeBudget = 10.Seconds()
                }
            };
            
            var service = Guid.NewGuid().ToString();
            var client = CreateClientWithServiceSettings(settings, "not-default", service);
            var cache = TimeoutSettingsProviderCache.Get(client, "not-default", service);
            var result = await cache.Get("", "");

            result.Should().Be(settings.Defaults.TimeBudget);
        }

        [Test]
        public async Task SettingsAliasProviderCache_should_detect_identity_correctly()
        {
            var settings = new SingularSettings
            {
                PathPatternSigns = new SingularSettings.PathPatternSettings
                {
                    Rules = new List<PathSettingsRule>
                    {
                        new PathSettingsRule
                        {
                            Method = "*",
                            PathPattern = "*",
                            TimeBudget = 10.Seconds()
                        }
                    }
                }
            };
            var service = Guid.NewGuid().ToString();
            var client = CreateClientWithServiceSettings(settings, "default", service);
            var cache = SettingsAliasProviderCache.Get(client, "default", service);
            var res = await cache.Get("", "");

            res.TimeBudget.Should().Be(10.Seconds());
        }

        private static IClusterClient CreateClientWithServiceSettings(SingularSettings settings, string environment, string service)
        {
            var client = Substitute.For<IClusterClient>();

            var versionedSettings = new VersionedSettings<SingularSettings>(SettingsVersionType.ClusterConfig, 1, settings);
            var content = ConfigurationPrinter.Print(versionedSettings, new PrintSettings {Format = PrintFormat.JSON});

            client.SendAsync(Arg.Is<Request>(x => x.Url.ToString().Contains(environment) && x.Url.ToString().Contains(service)))
                .Returns(new ClusterResult(ClusterResultStatus.Success,
                    new List<ReplicaResult>(),
                    new Response(ResponseCode.Ok, new Content(Encoding.UTF8.GetBytes(content))),
                    Request.Get("not-important")));

            return client;
        }
    }
}