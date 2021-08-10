using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vostok.Singular.Core
{
    [Serializable]
    public class RewriteSettings
    {
        public string Pattern = string.Empty;

        public string Replacement = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public RewriteMode Mode = RewriteMode.ByPrefix;
    }
}