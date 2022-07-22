using System.Collections.Generic;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class ClientIdempotencyRulesBuilder
    {
        private readonly List<(string method, string pattern, bool isIdempotent)> rules = new();

        public ClientIdempotencyRulesBuilder WithSign(string method, string pattern, bool idempotent = false)
        {
            rules.Add((method, pattern, idempotent));
            return this;
        }

        public IEnumerable<(string method, string pattern, bool isIdempotent)> Build() => rules;
    }
}