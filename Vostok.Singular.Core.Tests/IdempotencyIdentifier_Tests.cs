using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Clusterclient.Core.Model;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Extensions;
using Vostok.Singular.Core.PathPatterns.Idempotency;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.Tests
{
    public class IdempotencyIdentifier_Tests
    {
        private IBlackListIdempotencyResolver blackListResolver;
        private IIclResolver iclResolver;
        private IdempotencyIdentifier idempotencyIdentifier;
        private Request request;

        [SetUp]
        public void Setup()
        {
            blackListResolver = Substitute.For<IBlackListIdempotencyResolver>();
            iclResolver = Substitute.For<IIclResolver>();
            idempotencyIdentifier = new IdempotencyIdentifier(blackListResolver, iclResolver);
            request = Request.Get("test");
        }

        [Test]
        public async Task IsIdempotentAsync_should_take_value_from_header_if_exists()
        {
            iclResolver.IsIdempotentAsync("", "").ReturnsForAnyArgs(true);
            blackListResolver.IsIdempotent("", "").ReturnsForAnyArgs(true);
            request = request.WithHeader(SingularHeaders.Idempotent, false);

            var result = await idempotencyIdentifier.IsIdempotentAsync(request.Method, IdempotencySignBasedRequestStrategy.GetRequestUrl(request.Url), request.GetIdempotencyHeader());

            result.Should().BeFalse();
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_false_if_notIdempotent_only_in_BlackListResolver()
        {
            iclResolver.IsIdempotentAsync("", "").ReturnsForAnyArgs(true);
            blackListResolver.IsIdempotent("", "").ReturnsForAnyArgs(false);

            var result = await idempotencyIdentifier.IsIdempotentAsync(request.Method, IdempotencySignBasedRequestStrategy.GetRequestUrl(request.Url), request.GetIdempotencyHeader());

            result.Should().BeFalse();
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_false_if_notIdempotent_only_in_IclResolver()
        {
            iclResolver.IsIdempotentAsync("", "").ReturnsForAnyArgs(false);
            blackListResolver.IsIdempotent("", "").ReturnsForAnyArgs(true);

            var result = await idempotencyIdentifier.IsIdempotentAsync(request.Method, IdempotencySignBasedRequestStrategy.GetRequestUrl(request.Url), request.GetIdempotencyHeader());

            result.Should().BeFalse();
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_false_if_notIdempotent_in_both_resolvers()
        {
            iclResolver.IsIdempotentAsync("", "").ReturnsForAnyArgs(false);
            blackListResolver.IsIdempotent("", "").ReturnsForAnyArgs(false);

            var result = await idempotencyIdentifier.IsIdempotentAsync(request.Method, IdempotencySignBasedRequestStrategy.GetRequestUrl(request.Url), request.GetIdempotencyHeader());

            result.Should().BeFalse();
        }

        [Test]
        public async Task IsIdempotentAsync_should_return_true_if_idempotent_by_both_resolvers()
        {
            iclResolver.IsIdempotentAsync("", "").ReturnsForAnyArgs(true);
            blackListResolver.IsIdempotent("", "").ReturnsForAnyArgs(true);

            var result = await idempotencyIdentifier.IsIdempotentAsync(request.Method, IdempotencySignBasedRequestStrategy.GetRequestUrl(request.Url), request.GetIdempotencyHeader());

            result.Should().BeTrue();
        }
    }
}