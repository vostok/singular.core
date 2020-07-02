using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    internal class BriefClusterResult
    {
        public BriefClusterResult(ClusterResultStatus clusterResultStatus, IList<BriefReplicaResult> briefReplicaResults)
        {
            Status = clusterResultStatus;
            ReplicaResults = briefReplicaResults;
        }

        public BriefClusterResult(ClusterResult clusterResult)
        {
            Status = clusterResult.Status;
            ReplicaResults = clusterResult.ReplicaResults.Select(result => new BriefReplicaResult(result)).ToList();
        }

        public ClusterResultStatus Status { get; }
        public IList<BriefReplicaResult> ReplicaResults { get; }
    }
}