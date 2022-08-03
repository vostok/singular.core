using System.Collections.Generic;

namespace Vostok.Singular.Core.Tls
{
    internal interface IThumbprintVerificationSettingsProvider
    {
        bool AllowAnyThumbprintExceptBlacklisted { get; }
        IEnumerable<string> GetBlacklist();
        IEnumerable<string> GetWhitelist();
    }
}