using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Sending;
using Vostok.Clusterclient.Core.Strategies;
using Vostok.Singular.Core.Idempotency.Identifier;

namespace Vostok.Singular.Core
{
    internal class IdempotencySingBasedRequestStrategy : IRequestStrategy
    {
        private readonly IRequestStrategy sequential1Strategy;
        private readonly IRequestStrategy idempotencyStrategy;
        private IIdempotencyIdentifier notIdempotencyStrategy;

        public IdempotencySingBasedRequestStrategy(IIdempotencyIdentifier notIdempotencyStrategy, IRequestStrategy sequential1Strategy, IRequestStrategy idempotencyStrategy)
        {
            this.sequential1Strategy = sequential1Strategy;
            this.idempotencyStrategy = idempotencyStrategy;
            this.notIdempotencyStrategy = notIdempotencyStrategy;
        }

        public Task SendAsync(
            Request request, 
            RequestParameters parameters, 
            IRequestSender sender, 
            IRequestTimeBudget budget, 
            IEnumerable<Uri> replicas, 
            int replicasCount, 
            CancellationToken cancellationToken)
        {
            var url = request.Url;
            var path = url.IsAbsoluteUri ? url.AbsolutePath : url.OriginalString;
            
            var selectedStrategy = notIdempotencyStrategy.IsIdempotent(request.Method, path) ? idempotencyStrategy : sequential1Strategy;

            return selectedStrategy.SendAsync(request, parameters, sender, budget, replicas, replicasCount, cancellationToken);
        }
    }
}