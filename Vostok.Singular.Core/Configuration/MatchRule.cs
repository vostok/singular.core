using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class MatchRule
    {
        public SingularSettings.RejectTarget Target;

        public string KeyPattern;

        public string ValuePattern;

        public LogicalOperand LogicalOperand;

        public List<MatchRule> NestingRules;
    }
}