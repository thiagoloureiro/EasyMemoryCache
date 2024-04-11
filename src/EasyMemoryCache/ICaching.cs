using EasyMemoryCache.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache
{
    public interface ICaching
    {
        T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        void Invalidate(string key);

        void InvalidateAll();

        Task InvalidateAllAsync();

        void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes);

        object GetValueFromCache(string key);

        T GetValueFromCache<T>(string key);
        Task<T> GetValueFromCacheAsync<T>(string key);
        IEnumerable<string> GetKeys();
    }
}