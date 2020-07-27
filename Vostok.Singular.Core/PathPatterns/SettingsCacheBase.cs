using System;
using System.Collections.Generic;
using Vostok.Commons.Collections;

namespace Vostok.Singular.Core.PathPatterns
{
    //CR: (deniaa) 1. Why second generic parameter has name TRule? Why `rule` in `settingsCache`?
    //CR: (deniaa) 2. For what this class? This is only abstract wrapper over CachingTransform<TRaw, TProcessed>?
    //CR: (deniaa) You need to pass FactoryTRaw> in constructor and override Processor<TProcessed, TRaw> instead of..
    //CR: (deniaa) Put Factory<TRaw> into second argument and Processor<TProcessed, TRaw> into first one?
    //CR: (deniaa) What we save, 1 line of code with `cache` field and 1 line of creation this CachingTransform?
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