using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache
{
    public interface ICaching
    {
        T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool cacheEmptyList = false);

        Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false);

        void Invalidate(string key);

        void InvalidateAll();

        Task InvalidateAllAsync();

        void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120);

        Task SetValueToCacheAsync(string key, object value, int cacheTimeInMinutes = 120);

        object GetValueFromCache(string key);

        T GetValueFromCache<T>(string key);

        IEnumerable<string> GetKeys();
    }
}