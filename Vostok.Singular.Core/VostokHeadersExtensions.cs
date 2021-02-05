using System;
using JetBrains.Annotations;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core
{
    internal static class VostokHeadersExtensions
    {
        public static bool IsThrottledBySingularItSelf([NotNull] this Headers singularReplicaResponseHeaders)
        {
            var singularThrottlingHeader = singularReplicaResponseHeaders[SingularHeaders.SingularThrottlingTrigger];

            return string.Equals(singularThrottlingHeader, SingularHeaders.ThrottlingTriggerReason.ServerThrottlingQueueOverflow, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(singularThrottlingHeader, SingularHeaders.ThrottlingTriggerReason.ServerQuotaExhaustion, StringComparison.OrdinalIgnoreCase);
        }
    }
}