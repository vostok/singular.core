using System;

namespace Vostok.Singular.Core.Identifier
{
    internal static class IclRuleMatcher
    {
        public static bool IsMatch(IdempotencyControlRule rule, string method, string path)
        {
            if (rule.Method == null || rule.PathPattern == null)
                return false;

            if (!IsMethodMatched(rule, method) || path == null)
                return false;

            if (rule.PathPattern.IsMatch(path))
            {
                return true;
            }

            return false;
        }

        private static bool IsMethodMatched(IdempotencyControlRule rule, string method) =>
            string.Equals(rule.Method, method, StringComparison.OrdinalIgnoreCase)
            || rule.Method == "*";
    }
}