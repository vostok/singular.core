﻿using System;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns.PathRules
{
    internal class PathSettings : PathPatternRule
    {
        public TimeSpan? TimeBudget;

        public string SettingsAlias;

        public StrategySettings StrategySettings;

        public RedirectSettings RedirectSettings;
    }
}