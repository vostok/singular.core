using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Singular.Core.PathPatterns.Idempotency;

namespace Vostok.Singular.Core.Tests
{
    public class ClientIdempotencyRulesBuilder_Tests
    {
        [Test]
        public void Builder_should_save_rules_order()
        {
            var builder = new ClientIdempotencyRulesBuilder()
                .WithSign(RequestMethods.Get, "some/path", true)
                .WithSign(RequestMethods.Get, "another/path");

            var rules = builder.Build();

            rules.Should()
                .ContainInOrder(new List<(string, string, bool)>
                {
                    (RequestMethods.Get, "some/path", true),
                    (RequestMethods.Get, "another/path", false)
                });
        }
    }
}