﻿using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class MatchPattern
    {
        public SingularSettings.ConstrainTarget Target;

        public string KeyPattern;

        public string ValuePattern;

        public LogicalOperand LogicalOperand;

        public List<MatchPattern> NestingRules;
    }
    
}