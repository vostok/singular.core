using System.Collections.Generic;

namespace Vostok.Singular.Core.Tls
{
    internal interface IThumbprintVerificationSettingsProvider
    {
        IList<string> GetBlacklist();
        IList<string> GetWhitelist();
    }
}