using System.Collections.Generic;

namespace Vostok.Singular.Core.Tls
{
    internal interface IThumbprintsProvider
    {
        IEnumerable<string> GetBlacklist();
        IEnumerable<string> GetWhitelist();
    }
}