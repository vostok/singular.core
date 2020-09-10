using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Collections;
using Vostok.Commons.Threading;

namespace Vostok.Singular.Core
{
    internal class CachingTransformAsync<TRaw, TProcessed>
    {
        private readonly Func<Task<TRaw>> provider;
        private readonly Func<TRaw, TProcessed> processor;
        private readonly IEqualityComparer<TRaw> comparer;
        private readonly AsyncLock syncObject;

        private volatile Tuple<TRaw, TProcessed> cache;

        public CachingTransformAsync(
            Func<TRaw, TProcessed> processor,
            Func<Task<TRaw>> provider = null,
            IEqualityComparer<TRaw> comparer = null)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.provider = provider;
            this.comparer = comparer ?? SelectDefaultComparer();

            syncObject = new AsyncLock();
        }

        public async Task<TProcessed> GetAsync()
        {
            if (provider == null)
                throw new InvalidOperationException("Raw value provider delegate is not defined.");

            return await GetAsync(await provider().ConfigureAwait(false)).ConfigureAwait(false);
        }

        private async Task<TProcessed> GetAsync(TRaw raw)
        {
            var currentCache = cache;

            if (IsValidCache(currentCache, raw))
                return currentCache.Item2;

            using (await syncObject.LockAsync().ConfigureAwait(false))
            {
                if (IsValidCache(cache, raw))
                    return cache.Item2;

                var processed = processor(raw);

                Interlocked.Exchange(ref cache, Tuple.Create(raw, processed));

                return processed;
            }
        }

        private static IEqualityComparer<TRaw> SelectDefaultComparer()
        {
            if (typeof(TRaw).IsValueType)
                return EqualityComparer<TRaw>.Default;

            return ByReferenceEqualityComparer<TRaw>.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidCache(Tuple<TRaw, TProcessed> currentCache, TRaw actualRaw)
        {
            return currentCache != null && comparer.Equals(currentCache.Item1, actualRaw);
        }
    }
}