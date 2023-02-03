using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Sending;
using Vostok.Clusterclient.Core.Strategies;
using Vostok.Singular.Core.PathPatterns.Extensions;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    internal class SingularTimeoutSettingsStrategy : IRequestStrategy
    {
        private readonly IRequestStrategy requestStrategy;
        private readonly TimeoutSettingsProvider timeoutSettingsProvider;

        public SingularTimeoutSettingsStrategy(IRequestStrategy requestStrategy, TimeoutSettingsProvider timeoutSettingsProvider)
        {
            this.requestStrategy = requestStrategy;
            this.timeoutSettingsProvider = timeoutSettingsProvider;
        }

        public async Task SendAsync(
            Request request,
            RequestParameters parameters,
            IRequestSender sender,
            IRequestTimeBudget budget,
            IEnumerable<Uri> replicas,
            int replicasCount,
            CancellationToken cancellationToken)
        {
            var timeout = await timeoutSettingsProvider.Get(request.Method, request.Url.GetRequestPath()).ConfigureAwait(false);

            var newBudget = RequestTimeBudget.StartNew(timeout);

            await requestStrategy.SendAsync(request, parameters, sender, newBudget, replicas, replicasCount, cancellationToken).ConfigureAwait(false);
        }

        public override string ToString() => "SingularStrategy";
    }
}