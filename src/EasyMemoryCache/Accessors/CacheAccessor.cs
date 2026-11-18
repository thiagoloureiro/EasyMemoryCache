using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache.Accessors
{
    public abstract class CacheAccessor : IDisposable
    {
        private bool _disposed;
        protected readonly IDistributedCache Cache;

        protected CacheAccessor(IDistributedCache cache)
        {
            Cache = cache;
        }

        public abstract object Get(string key);

        public abstract T Get<T>(string key);

        public abstract Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        public abstract bool TryGet<T>(string key, out T value);

        public abstract Task<CacheResult<T>> TryGetAsync<T>(string key, CancellationToken cancellationToken = default);

        protected abstract void SetInternal(string key, object value, DistributedCacheEntryOptions options);

        protected abstract Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken cancellationToken);

        public void Set(string key, object value, int cacheTime)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var entryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheTime)
            };

            SetInternal(key, value, entryOptions);
        }

        public async Task SetAsync(string key, object value, int cacheTime, CancellationToken cancellationToken = default)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var entryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheTime)
            };

            await SetInternalAsync(key, value, entryOptions, cancellationToken).ConfigureAwait(false);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return Cache.RemoveAsync(key, cancellationToken);
        }

        public void Dispose() => Dispose(true);

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ((RedisCache)Cache)?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}