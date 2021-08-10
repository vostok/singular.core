using System;
using Vostok.Clusterclient.Core.Ordering.Weighed.Relative;
using Vostok.Commons.Time;

namespace Vostok.Singular.Core
{
    [Serializable]
    public class LocalWeightSettings
    {
        public double InitialWeight = 1.0d;
        public double MinWeight = 0.001d;
        public int PenaltyMultiplier = 100;
        public int WeightByStatusesRpsThreshold = 10;
        public double Sensitivity = 4;
        public double RegenerationRatePerMinute = 0.05;
        public TimeSpan RegenerationLag = 1.Minutes();
        public TimeSpan StatisticSmoothingConstant = 1.Seconds();
        public TimeSpan StatisticTTL = 10.Minutes();
        public TimeSpan WeightUpdatePeriod = 10.Seconds();
        public TimeSpan WeightsDownSmoothingConstant = 1.Seconds();
        public TimeSpan WeightsRaiseSmoothingConstant = 1.Minutes();
        public TimeSpan WeightsTTL = 5.Minutes();

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
                WeightsTTL = WeightsTTL
            };
        }
    }
}