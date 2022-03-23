using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;

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
        public abstract Task<T> GetAsync<T>(string key);
        protected abstract void SetInternal(string key, object value, DistributedCacheEntryOptions options);
        protected abstract Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options);

        public void Set(string key, object value, int cacheTimeInMinutes = 120)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var entryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
            };

            SetInternal(key, value, entryOptions);
        }

        public async Task SetAsync(string key, object value, int cacheTimeInMinutes = 120)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var entryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
            };

            await SetInternalAsync(key, value, entryOptions);
        }

        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        public void RemoveAsync(string key)
        {
            Cache.RemoveAsync(key);
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
