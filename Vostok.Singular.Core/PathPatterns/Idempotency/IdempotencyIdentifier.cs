using System.Threading.Tasks;
using Vostok.Singular.Core.PathPatterns.BlackList;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly IBlackListIdempotencyResolver blackListIdempotencyResolver;
        private readonly IIclResolver iclRulesProvider;

        public IdempotencyIdentifier(
            IBlackListIdempotencyResolver blackListIdempotencyResolver,
            IIclResolver iclRulesProvider)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclRulesProvider = iclRulesProvider;
        }

        public async Task<bool> IsIdempotentAsync(string method, string path, string headerValue)
        {
            var rule = await iclRulesProvider.GetRuleAsync(method, path).ConfigureAwait(false);

            if (rule.OverrideHeader && bool.TryParse(headerValue, out var value))
                return value;
            
            return await blackListIdempotencyResolver.IsIdempotent(method, path).ConfigureAwait(false) && rule.IsIdempotent;
        }
    }
}