using System;

namespace Vostok.Singular.Core
{
    [Serializable]
    public class StrategySettings
    {
        public string RequestStrategy;

        public int? MaxForkingParallelism;

        public TimeSpan? MinForkingDelay;

        public int? MaxParallelParallelism;

        public int? SequentialDivisionFactor;
    }
}