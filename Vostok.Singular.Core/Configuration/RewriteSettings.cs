using System;

namespace Vostok.Singular.Core
{
    [Serializable]
    internal class RewriteSettings
    {
        public string Pattern = string.Empty;

        public string Replacement = string.Empty;

        public RewriteMode Mode = RewriteMode.ByPrefix;
    }
}