using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class PathPatternSettings
    {
        public List<PathSettingsRule> Rules = new List<PathSettingsRule>();
    }
}