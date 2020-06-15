using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal class BriefClusterResult
    {
        public ClusterResultStatus Status;
        public IList<BriefReplicaResult> ReplicaResults;

        public BriefClusterResult(ClusterResultStatus clusterResultStatus, IList<BriefReplicaResult> briefReplicaResults)
        {
            Status = clusterResultStatus;
            ReplicaResults = briefReplicaResults;
        }

        public BriefClusterResult(ClusterResult vostokClusterResult)
        {
            Status = vostokClusterResult.Status;
            ReplicaResults = vostokClusterResult.ReplicaResults.Select(result => new BriefReplicaResult(result)).ToList();
        }
    }
}