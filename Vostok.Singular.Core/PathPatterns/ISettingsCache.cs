using System.Collections.Generic;

namespace Vostok.Singular.Core.PathPatterns
{
    internal interface ISettingsCache<TSettings>
    {
        List<TSettings> Get();
    }
}