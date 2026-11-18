using AsyncKeyedLock;
using EasyMemoryCache.Configuration;
using EasyMemoryCache.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache.Memorycache
{
    public class Caching : ICaching, IDisposable
    {
        private readonly MemoryCache _myCache;
        private readonly bool _ownsCache;
        private int _disposed;

        private readonly AsyncKeyedLocker<string> _cacheLock = new AsyncKeyedLocker<string>(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        public Caching() : this(new MemoryCache(new MemoryCacheOptions()), ownsCache: true)
        {
        }

        [ActivatorUtilitiesConstructor]
        public Caching(IMemoryCache cache) : this(cache, ownsCache: false)
        {
        }

        private Caching(IMemoryCache cache, bool ownsCache)
        {
            if (cache is not MemoryCache memoryCache)
                throw new ArgumentException("The cache must be a MemoryCache instance.", nameof(cache));

            _myCache = memoryCache;
            _ownsCache = ownsCache;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);

            if (TryRead(cacheItemName, out T cachedObject))
                return cachedObject;

            using (_cacheLock.Lock(cacheItemName))
            {
                if (TryRead(cacheItemName, out cachedObject))
                    return cachedObject;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                cachedObject = objectSettingFunction();
                Store(cacheItemName, cachedObject, ttlSeconds, cacheEmptyList);
                return cachedObject;
            }
        }

        public Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            return GetOrSetObjectFromCacheAsync(cacheItemName, cacheTime, _ => objectSettingFunction(), cacheEmptyList, interval, CancellationToken.None);
        }

        public Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<CancellationToken, Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            CachePolicies.RequireKey(cacheItemName, nameof(cacheItemName));
            ArgumentNullException.ThrowIfNull(objectSettingFunction);
            cancellationToken.ThrowIfCancellationRequested();

            if (TryRead(cacheItemName, out T cachedObject))
                return Task.FromResult(cachedObject);

            return GetOrSetUnderLockAsync(cacheItemName, cacheTime, objectSettingFunction, cacheEmptyList, interval, cancellationToken);
        }

        public void Invalidate(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            _myCache.Remove(key);
        }

        public Task InvalidateAsync(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Invalidate(key);
            return Task.CompletedTask;
        }

        public void InvalidateAll()
        {
            _myCache.Clear();
        }

        public Task InvalidateAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            InvalidateAll();
            return Task.CompletedTask;
        }

        public void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            CachePolicies.RequireKey(key, nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            _myCache.Set(key, value, EntryOptions(cacheTime, interval));
        }

        public Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SetValueToCache(key, value, cacheTime, interval);
            return Task.CompletedTask;
        }

        public object GetValueFromCache(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _myCache.Get(key);
        }

        public T GetValueFromCache<T>(string key)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return _myCache.Get<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            CachePolicies.RequireKey(key, nameof(key));
            return TryRead(key, out value);
        }

        public Task<T> GetValueFromCacheAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(GetValueFromCache<T>(key));
        }

        public Task<CacheResult<T>> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(TryGetValue(key, out T value)
                ? CacheResult<T>.Hit(value)
                : CacheResult<T>.Miss);
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
                return;

            _cacheLock.Dispose();
            if (_ownsCache)
                _myCache.Dispose();
        }

        public IEnumerable<string> GetKeys()
        {
            var keys = _myCache.GetKeys();
            var lst = new List<string>();
            foreach (var key in keys)
            {
                lst.Add(key.ToString());
            }
            return lst;
        }

#pragma warning disable CS1998
        public async IAsyncEnumerable<string> GetKeysAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var key in GetKeys())
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return key;
            }
        }
#pragma warning restore CS1998

        private async Task<T> GetOrSetUnderLockAsync<T>(string cacheItemName, int cacheTime, Func<CancellationToken, Task<T>> objectSettingFunction, bool cacheEmptyList, CacheTimeInterval interval, CancellationToken cancellationToken)
        {
            using (await _cacheLock.LockAsync(cacheItemName, cancellationToken).ConfigureAwait(false))
            {
                if (TryRead(cacheItemName, out T cachedObject))
                    return cachedObject;

                var ttlSeconds = ConvertInterval.Convert(interval, cacheTime);
                cachedObject = await objectSettingFunction(cancellationToken).ConfigureAwait(false);
                Store(cacheItemName, cachedObject, ttlSeconds, cacheEmptyList);
                return cachedObject;
            }
        }

        private bool TryRead<T>(string key, out T value)
        {
            if (_myCache.TryGetValue(key, out object raw))
            {
                value = (T)raw;
                return true;
            }

            value = default;
            return false;
        }

        private void Store<T>(string key, T value, int ttlSeconds, bool cacheEmptyList)
        {
            if (!CachePolicies.ShouldStore(value, cacheEmptyList))
                return;

            _myCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds)
            });
        }

        private static MemoryCacheEntryOptions EntryOptions(int cacheTime, CacheTimeInterval interval)
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ConvertInterval.Convert(interval, cacheTime))
            };
        }
    }
}
