using EasyMemoryCache.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache
{
    public interface ICaching
    {
        T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<CancellationToken, Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default);

        void Invalidate(string key);

        Task InvalidateAsync(string key, CancellationToken cancellationToken = default);

        void InvalidateAll();

        Task InvalidateAllAsync(CancellationToken cancellationToken = default);

        void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes, CancellationToken cancellationToken = default);

        object GetValueFromCache(string key);

        T GetValueFromCache<T>(string key);

        bool TryGetValue<T>(string key, out T value);

        Task<T> GetValueFromCacheAsync<T>(string key, CancellationToken cancellationToken = default);

        Task<CacheResult<T>> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default);

        IEnumerable<string> GetKeys();

        IAsyncEnumerable<string> GetKeysAsync(CancellationToken cancellationToken = default);
    }
}
