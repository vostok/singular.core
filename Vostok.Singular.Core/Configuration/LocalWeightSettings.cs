using System;
using Vostok.Clusterclient.Core.Ordering.Weighed.Relative;

namespace Vostok.Singular.Core
{
    [Serializable]
    internal class LocalWeightSettings
    {
        public bool WeightsOnlyByStatuses;
        public double InitialWeight = 1.0d;
        public double MinWeight = 0.001d;
        public int PenaltyMultiplier = 100;
        public int WeightByStatusesRpsThreshold = 10;
        public double Sensitivity = 4;
        public double RegenerationRatePerMinute = 0.05;
        public TimeSpan RegenerationLag = TimeSpan.FromMinutes(1);
        public TimeSpan StatisticSmoothingConstant = TimeSpan.FromSeconds(1);
        public TimeSpan StatisticTTL = TimeSpan.FromMinutes(10);
        public TimeSpan WeightUpdatePeriod = TimeSpan.FromSeconds(10);
        public TimeSpan WeightsDownSmoothingConstant = TimeSpan.FromSeconds(1);
        public TimeSpan WeightsRaiseSmoothingConstant = TimeSpan.FromMinutes(1);
        public TimeSpan WeightsTTL = TimeSpan.FromMinutes(5);
        public double SignificantWeightChangeToLog = 0.1;
        public double DegradedWeightBorderToLog = 0.7;

        public RelativeWeightSettings ToRelativeWeightSettings()
        {
            return new RelativeWeightSettings()
            {
                WeightUpdatePeriod = WeightUpdatePeriod,
                WeightByStatusesRpsThreshold = WeightByStatusesRpsThreshold,
                StatisticSmoothingConstant = StatisticSmoothingConstant,
                MinWeight = MinWeight,
                InitialWeight = InitialWeight,
                WeightsDownSmoothingConstant = WeightsDownSmoothingConstant,
                PenaltyMultiplier = PenaltyMultiplier,
                RegenerationLag = RegenerationLag,
                RegenerationRatePerMinute = RegenerationRatePerMinute,
                Sensitivity = Sensitivity,
                StatisticTTL = StatisticTTL,
                WeightsRaiseSmoothingConstant = WeightsRaiseSmoothingConstant,
                WeightsTTL = WeightsTTL,
                WeightsOnlyByStatuses = WeightsOnlyByStatuses,
                DegradedWeightBorderToLog = DegradedWeightBorderToLog,
                SignificantWeightChangeToLog = SignificantWeightChangeToLog
            };
        }
    }
}