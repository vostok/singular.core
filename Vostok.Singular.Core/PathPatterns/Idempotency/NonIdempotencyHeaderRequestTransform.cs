using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Transforms;
using Vostok.Singular.Core.PathPatterns.Extensions;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class NonIdempotencyHeaderRequestTransform : IRequestTransform
    {
        private readonly IReadOnlyList<IdempotencyControlRule> rules;

        public NonIdempotencyHeaderRequestTransform(Func<ClientIdempotencyRulesBuilder, ClientIdempotencyRulesBuilder> buildRules)
        {
            if (buildRules is null)
                return;

            rules = buildRules(new ClientIdempotencyRulesBuilder())
                .Build()
                .Select(x => new IdempotencyControlRule
                {
                    Method = x.method, PathPattern = new Wildcard(x.pattern), IsIdempotent = x.isIdempotent
                })
                .ToArray();
        }

        public Request Transform(Request request)
        {
            if (!rules.Any())
                return request;

            var path = request.Url
                .GetRequestPath()
                .TrimStart('/');

            var rule = rules.FirstOrDefault(x => PathPatternRuleMatcher.IsMatch(x, request.Method, path));

            if (rule is null)
                return request;

            return rule.IsIdempotent ? request.WithIdempotentHeader() : request.WithNotIdempotentHeader();
        }
    }
}