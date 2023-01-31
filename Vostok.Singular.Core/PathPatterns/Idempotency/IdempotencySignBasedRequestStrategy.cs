using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core.Model;
using Vostok.Clusterclient.Core.Sending;
using Vostok.Clusterclient.Core.Strategies;
using Vostok.Singular.Core.PathPatterns.Extensions;

namespace Vostok.Singular.Core.PathPatterns.Idempotency
{
    internal class IdempotencySignBasedRequestStrategy : IRequestStrategy
    {
        private readonly IRequestStrategy sequential1Strategy;
        private readonly IRequestStrategy forkingStrategy;
        private readonly IIdempotencyIdentifier idempotencyIdentifier;

        public IdempotencySignBasedRequestStrategy(IIdempotencyIdentifier idempotencyIdentifier, IRequestStrategy sequential1Strategy, IRequestStrategy forkingStrategy)
        {
            this.sequential1Strategy = sequential1Strategy;
            this.forkingStrategy = forkingStrategy;
            this.idempotencyIdentifier = idempotencyIdentifier;
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
            var requestIsIdempotent = await idempotencyIdentifier
                .IsIdempotentAsync(request.Method, request.Url.GetRequestPath(), request.GetIdempotencyHeader())
                .ConfigureAwait(false);
            var selectedStrategy = requestIsIdempotent ? forkingStrategy : sequential1Strategy;

            await selectedStrategy.SendAsync(request, parameters, sender, budget, replicas, replicasCount, cancellationToken).ConfigureAwait(false);
        }
        
        //TODO (lunev.d 31.01.2023): Remove after releases of server and client modules 
        public static string GetRequestPath(Uri url)
        {
            if (url.IsAbsoluteUri)
                return url.AbsolutePath;

            var originalString = url.OriginalString;
            var queryIndex = originalString.IndexOf('?');
            return queryIndex > -1 ? originalString.Substring(0, queryIndex) : originalString;
        }
    }
}