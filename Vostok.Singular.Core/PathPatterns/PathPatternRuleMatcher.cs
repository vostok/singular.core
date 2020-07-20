using System;

namespace Vostok.Singular.Core.PathPatterns
{
    internal static class PathPatternRuleMatcher
    {
        public static bool IsMatch(PathPatternRule rule, string method, string path)
        {
            if (rule.Method == null || rule.PathPattern == null)
                return false;

            if (!IsMethodMatched(rule, method) || path == null)
                return false;

            return rule.PathPattern.IsMatch(path);
        }

        private static bool IsMethodMatched(PathPatternRule rule, string method) =>
            string.Equals(rule.Method, method, StringComparison.OrdinalIgnoreCase) || rule.Method == "*";
    }
}