using System;

namespace Vostok.Singular.Core.PathPatterns.Extensions
{
    internal static class UriExtensions
    {
        public static string GetRequestPath(this Uri url)
        {
            if (url.IsAbsoluteUri)
                return url.AbsolutePath;

            var originalString = url.OriginalString;
            var queryIndex = originalString.IndexOf('?');
            return queryIndex > -1 ? originalString.Substring(0, queryIndex) : originalString;
        }
    }
}