using AsyncKeyedLock;
using EasyMemoryCache.Configuration;
using EasyMemoryCache.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache.Memcached
{
    public class MemcachedCaching : ICaching, IDisposable
    {
        private readonly IMemcachedClient _memcachedClient;
        private int _disposed;

        private readonly AsyncKeyedLocker<string> _cacheLock = new AsyncKeyedLocker<string>(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        public MemcachedCaching(IMemcachedClient memcachedClient)
        {
            _memcachedClient = memcachedClient;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction,
            bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);

            if (_memcachedClient.TryGet(cacheItemName, out T cachedObject))
                return cachedObject;

            using (_cacheLock.Lock(cacheItemName))
            {
                if (_memcachedClient.TryGet(cacheItemName, out cachedObject))
                    return cachedObject;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                cachedObject = objectSettingFunction();
                if (CachePolicies.ShouldStore(cachedObject, cacheEmptyList))
                    _memcachedClient.Set(cacheItemName, cachedObject, ttlSeconds);

                return cachedObject;
            }
        }

        public Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction,
            bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            return GetOrSetObjectFromCacheAsync(cacheItemName, cacheTime, _ => objectSettingFunction(), cacheEmptyList, interval, CancellationToken.None);
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<CancellationToken, Task<T>> objectSettingFunction,
            bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            cancellationToken.ThrowIfCancellationRequested();

            var cached = await ReadAsync<T>(cacheItemName, cancellationToken).ConfigureAwait(false);
            if (cached.Found)
                return cached.Value;

            using (await _cacheLock.LockAsync(cacheItemName, cancellationToken).ConfigureAwait(false))
            {
                cached = await ReadAsync<T>(cacheItemName, cancellationToken).ConfigureAwait(false);
                if (cached.Found)
                    return cached.Value;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                var created = await objectSettingFunction(cancellationToken).ConfigureAwait(false);
                if (CachePolicies.ShouldStore(created, cacheEmptyList))
                    await _memcachedClient.SetAsync(cacheItemName, created, ttlSeconds).ConfigureAwait(false);

                return created;
            }
        }

        public void Invalidate(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            _memcachedClient.Remove(key);
        }

        public async Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            cancellationToken.ThrowIfCancellationRequested();
            await _memcachedClient.RemoveAsync(key).ConfigureAwait(false);
        }

        public void InvalidateAll()
        {
            _memcachedClient.FlushAll();
        }

        public async Task InvalidateAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _memcachedClient.FlushAllAsync().ConfigureAwait(false);
        }

        public void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            _memcachedClient.Set(key, value, ConvertInterval.Convert(interval, cacheTime));
        }

        public async Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            cancellationToken.ThrowIfCancellationRequested();
            await _memcachedClient.SetAsync(key, value, ConvertInterval.Convert(interval, cacheTime)).ConfigureAwait(false);
        }

        public object GetValueFromCache(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _memcachedClient.Get(key);
        }

        public T GetValueFromCache<T>(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _memcachedClient.Get<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _memcachedClient.TryGet(key, out value);
        }

        public async Task<T> GetValueFromCacheAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            cancellationToken.ThrowIfCancellationRequested();
            return await _memcachedClient.GetValueAsync<T>(key).ConfigureAwait(false);
        }

        public async Task<CacheResult<T>> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return await ReadAsync<T>(key, cancellationToken).ConfigureAwait(false);
        }

        public IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<string> GetKeysAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            _cacheLock.Dispose();
        }

        private async Task<CacheResult<T>> ReadAsync<T>(string key, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await _memcachedClient.GetAsync<T>(key).ConfigureAwait(false);
            return result.Success
                ? CacheResult<T>.Hit(result.Value)
                : CacheResult<T>.Miss;
        }
    }
}
