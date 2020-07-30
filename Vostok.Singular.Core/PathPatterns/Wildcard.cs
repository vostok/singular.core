using System.Text.RegularExpressions;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class Wildcard : Regex
    {
        public Wildcard(string pattern)
            : base(ConvertPattern(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline)
        {
        }

        private static string ConvertPattern(string pattern)
        {
            pattern = Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".");
            return "^" + pattern + "$";
        }
    }
}