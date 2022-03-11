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

        Task InvalidateAsync(string key);

        void InvalidateAll();

        Task InvalidateAllAsync();

        void SetValueToCache<T>(string key, T value, int cacheTimeInMinutes = 120);

        T GetValueFromCache<T>(string key);

        IEnumerable<string> GetKeys();

        IEnumerable<DataContainer> GetData();
    }
}