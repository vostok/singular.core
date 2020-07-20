using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.PathPatterns
{
    internal abstract class SettingsCacheBase<TSettings, TRule> : ISettingsCache<TRule>
    {
        private readonly CachingTransform<TSettings, List<TRule>> cache;

        internal SettingsCacheBase(Func<TSettings> settingsProvider)
        {
            cache = new CachingTransform<TSettings, List<TRule>>(PreprocessSettings, settingsProvider);
        }

        public List<TRule> Get()
        {
            return cache.Get();
        }

        protected abstract List<TRule> PreprocessSettings(TSettings settings);
    }
}