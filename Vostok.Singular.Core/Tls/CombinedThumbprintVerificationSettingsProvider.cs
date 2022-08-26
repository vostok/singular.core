using System.Collections.Generic;
using System.Linq;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.Tls
{
    internal class CombinedThumbprintVerificationSettingsProvider : IThumbprintVerificationSettingsProvider
    {
        private readonly IThumbprintVerificationSettingsProvider first;
        private readonly IThumbprintVerificationSettingsProvider second;
        private readonly CachingTransform<(IList<string>, IList<string>), IList<string>> blacklistTransform;
        private readonly CachingTransform<(IList<string>, IList<string>), IList<string>> whitelistTransform;

        public CombinedThumbprintVerificationSettingsProvider(IThumbprintVerificationSettingsProvider first, IThumbprintVerificationSettingsProvider second)
        {
            this.first = first;
            this.second = second;
            blacklistTransform = new CachingTransform<(IList<string>, IList<string>), IList<string>>(Concat);
            whitelistTransform = new CachingTransform<(IList<string>, IList<string>), IList<string>>(Concat);
        }

        public IList<string> GetBlacklist() => blacklistTransform.Get((first.GetBlacklist(), second.GetBlacklist()));

        public IList<string> GetWhitelist() => whitelistTransform.Get((first.GetWhitelist(), second.GetWhitelist()));

        private static IList<string> Concat((IList<string>, IList<string>) x) => x.Item1.Concat(x.Item2).ToList();
    }
}