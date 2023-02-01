using System;
using Vostok.Clusterclient.Core.Model;
using Vostok.Commons.Time;

namespace Vostok.Singular.Core.PathPatterns.Timeout
{
    // note (lunev.d, 02.02.2023): Can't use the original class because it's internal
    internal class RequestTimeBudget : TimeBudget, IRequestTimeBudget
    {
        private static readonly TimeSpan BudgetPrecision = TimeSpan.FromMilliseconds(15);

        public new static RequestTimeBudget Infinite = new RequestTimeBudget(TimeSpan.MaxValue, TimeSpan.Zero);

        private RequestTimeBudget(TimeSpan budget, TimeSpan precision)
            : base(budget, precision)
        {
        }

        public new static RequestTimeBudget StartNew(TimeSpan budget, TimeSpan? precision = null)
        {
            var timeBudget = new RequestTimeBudget(budget, precision ?? BudgetPrecision);
            timeBudget.Start();
            return timeBudget;
        }
    }
}