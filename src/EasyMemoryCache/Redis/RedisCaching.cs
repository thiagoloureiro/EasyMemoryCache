using AsyncKeyedLock;
using EasyMemoryCache.Accessors;
using EasyMemoryCache.Configuration;
using EasyMemoryCache.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache.Redis
{
    public class RedisCaching : ICaching, IDisposable
    {
        private readonly CacheAccessor _cacheAccessor;
        private readonly IServer _server;
        private int _disposed;

        private readonly AsyncKeyedLocker<string> _cacheLock = new AsyncKeyedLocker<string>(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        public RedisCaching(CacheAccessor cacheAccessor, IServer server)
        {
            _cacheAccessor = cacheAccessor;
            _server = server;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);

            if (_cacheAccessor.TryGet(cacheItemName, out T cachedObject))
                return cachedObject;

            using (_cacheLock.Lock(cacheItemName))
            {
                if (_cacheAccessor.TryGet(cacheItemName, out cachedObject))
                    return cachedObject;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                cachedObject = objectSettingFunction();
                if (CachePolicies.ShouldStore(cachedObject, cacheEmptyList))
                    _cacheAccessor.Set(cacheItemName, cachedObject, ttlSeconds);

                return cachedObject;
            }
        }

        public Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            return GetOrSetObjectFromCacheAsync(cacheItemName, cacheTime, _ => objectSettingFunction(), cacheEmptyList, interval, CancellationToken.None);
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<CancellationToken, Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            cancellationToken.ThrowIfCancellationRequested();

            var cached = await _cacheAccessor.TryGetAsync<T>(cacheItemName, cancellationToken).ConfigureAwait(false);
            if (cached.Found)
                return cached.Value;

            using (await _cacheLock.LockAsync(cacheItemName, cancellationToken).ConfigureAwait(false))
            {
                cached = await _cacheAccessor.TryGetAsync<T>(cacheItemName, cancellationToken).ConfigureAwait(false);
                if (cached.Found)
                    return cached.Value;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                var created = await objectSettingFunction(cancellationToken).ConfigureAwait(false);
                if (CachePolicies.ShouldStore(created, cacheEmptyList))
                    await _cacheAccessor.SetAsync(cacheItemName, created, ttlSeconds, cancellationToken).ConfigureAwait(false);

                return created;
            }
        }

        public void Invalidate(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            _cacheAccessor.Remove(key);
        }

        public Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            cancellationToken.ThrowIfCancellationRequested();
            return _cacheAccessor.RemoveAsync(key, cancellationToken);
        }

        public void InvalidateAll()
        {
            _server.FlushDatabase();
        }

        public Task InvalidateAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _server.FlushDatabaseAsync();
        }

        public void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            _cacheAccessor.Set(key, value, ConvertInterval.Convert(interval, cacheTime));
        }

        public Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            cancellationToken.ThrowIfCancellationRequested();
            return _cacheAccessor.SetAsync(key, value, ConvertInterval.Convert(interval, cacheTime), cancellationToken);
        }

        public object GetValueFromCache(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _cacheAccessor.Get(key);
        }

        public async Task<T> GetValueFromCacheAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            cancellationToken.ThrowIfCancellationRequested();
            return await _cacheAccessor.GetAsync<T>(key, cancellationToken).ConfigureAwait(false);
        }

        public T GetValueFromCache<T>(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _cacheAccessor.Get<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _cacheAccessor.TryGet(key, out value);
        }

        public Task<CacheResult<T>> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(key, nameof(key));
            cancellationToken.ThrowIfCancellationRequested();
            return _cacheAccessor.TryGetAsync<T>(key, cancellationToken);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            _cacheLock.Dispose();
            _cacheAccessor.Dispose();
        }

        public IEnumerable<string> GetKeys()
        {
            return _server.Keys().Select(x => x.ToString());
        }

        public async IAsyncEnumerable<string> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var key in _server.KeysAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return key.ToString();
            }
        }
    }
}
