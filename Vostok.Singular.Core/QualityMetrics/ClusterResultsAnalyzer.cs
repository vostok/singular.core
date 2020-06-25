using System.Collections.Generic;
using System.Linq;
using Vostok.Clusterclient.Core.Model;

namespace Vostok.Singular.Core.QualityMetrics
{
    // CR: Почему все методы называются просто Find find find? 
    // CR: Где-то FindSource, где-то FindReason, а возвращают одно и то же...

    // CR: Еще напишу тут же. В module.yaml как-то попал clusterclient.core, это не собирается.
    // CR: А в src-конфигурации не хватает новых добавленных модулей, так ничего не соберется у клиентов.
    // CR: vostok.commons.time стоит подключать в конфигурации src.
    // CR: Для того, чтобы в клиентах не писать много include строк для файлов из Vostok.Singular.Core.QualityMetrics, можно было бы сделать отдельный props с инклюдами.

    internal static class ClusterResultsAnalyzer
    {
        private static readonly Dictionary<ResponseCode, ResultReason> ReplicaExhaustedReasons = new Dictionary<ResponseCode, ResultReason>
        {
            {ResponseCode.Unknown, ResultReason.Unknown},
            {ResponseCode.RequestTimeout, ResultReason.RequestTimeout},
            {ResponseCode.ConnectFailure, ResultReason.ConnectFailure},
            {ResponseCode.ReceiveFailure, ResultReason.ReceiveFailure},
            {ResponseCode.SendFailure, ResultReason.SendFailure},
            {ResponseCode.InternalServerError, ResultReason.InternalServerError},
            {ResponseCode.ServiceUnavailable, ResultReason.ServiceUnavailable},
            {ResponseCode.ProxyTimeout, ResultReason.ProxyTimeout}, // CR: Эта пара тут лишняя, потому что будет DontRetryHeader, а это AcceptNonRetriableCriterion.
            {ResponseCode.UnknownFailure, ResultReason.UnknownFailure},
            {ResponseCode.StreamReuseFailure, ResultReason.StreamReuseFailure},
            {ResponseCode.StreamInputFailure, ResultReason.StreamInputFailure}
        };

        // CR: Зачем тащить в старого Сингуляр-клиента зависимость от восточного КК? 
        // CR: Какие-то сомнительные перекладывания восточного результата в другой класс, на мой взгляд.
        // CR: Предлагаю обсудить варианты, можно ли это по-другому сделать.
        public static ResultReason FindResultSource(ClusterResult result) =>
            FindResultSource(new BriefClusterResult(result));

        public static ResultReason FindResultSource(BriefClusterResult result)
        {
            switch (result.Status)
            {
                case ClusterResultStatus.TimeExpired:
                    return ResultReason.TimeExpired;
                case ClusterResultStatus.ReplicasNotFound:
                    return ResultReason.SingularReplicasNotFound;
                case ClusterResultStatus.UnexpectedException:
                    return ResultReason.UnexpectedException;
                case ClusterResultStatus.ReplicasExhausted:
                    return FindResultReason(result.ReplicaResults);
                default:
                    return ResultReason.Backend;
            }
        }

        private static ResultReason FindResultReason(IList<BriefReplicaResult> replicaResults)
        {
            var severalDifferentReasons = false;
            ResultReason? lastReason = null;
            foreach (var replicaResult in replicaResults.Select(FindResultReason))
            {
                if (replicaResult == ResultReason.Backend)
                    return ResultReason.Backend;
                if (lastReason == null || lastReason == replicaResult)
                    lastReason = replicaResult;
                else
                    severalDifferentReasons = true;  // CR: return ResultReason.Complex?
            }

            if (severalDifferentReasons)
                return ResultReason.Complex;
            
            return lastReason ?? ResultReason.Backend;
        }

        private static ResultReason FindResultReason(BriefReplicaResult replicaResult)
        {
            // CR: Если оно HasBackendHeader, то это уже не будет ClusterResultStatus.ReplicasExhausted.
            if(replicaResult.Code == ResponseCode.BadGateway || replicaResult.HasBackendHeader)
                return ResultReason.Backend;
            if (ReplicaExhaustedReasons.TryGetValue(replicaResult.Code, out var resultReason))
                return resultReason;
            if (replicaResult.Code == ResponseCode.TooManyRequests && replicaResult.HasIsSingularThrottlingTriggerHeader)
                return ResultReason.SingularThrottling;

            return ResultReason.Backend;
        }
    }
}