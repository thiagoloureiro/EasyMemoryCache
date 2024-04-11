using EasyMemoryCache.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Extensions;

namespace EasyMemoryCache.Memcached
{
    public class MemcachedCaching : ICaching
    {
        private readonly IMemcachedClient _memcachedClient;

        public MemcachedCaching(IMemcachedClient memcachedClient)
        {
            _memcachedClient = memcachedClient;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTime, Func<T> objectSettingFunction,
            bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            T result;

            var data = _memcachedClient.Get(cacheItemName);
            if (data == null)
            {
                result = objectSettingFunction();
                _memcachedClient.Set(cacheItemName, result, ConvertInterval.Convert(interval, cacheTime));
            }
            else
            {
                return (T)data;
            }
            return result;
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTime, Func<Task<T>> objectSettingFunction,
            bool cacheEmptyList = false, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            return await _memcachedClient.GetValueOrCreateAsync(cacheItemName, ConvertInterval.Convert(interval, cacheTime),
                objectSettingFunction);
        }

        public void Invalidate(string key)
        {
            _memcachedClient.Remove(key);
        }

        public void InvalidateAll()
        {
            _memcachedClient.FlushAll();
        }

        public async Task InvalidateAllAsync()
        {
            await _memcachedClient.FlushAllAsync();
        }

        public void SetValueToCache(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            _memcachedClient.Add(key, value, ConvertInterval.Convert(interval, cacheTime));
        }

        public async Task SetValueToCacheAsync(string key, object value, int cacheTime = 120, CacheTimeInterval interval = CacheTimeInterval.Minutes)
        {
            await _memcachedClient.AddAsync(key, value, ConvertInterval.Convert(interval, cacheTime)).ConfigureAwait(false);
        }

        public object GetValueFromCache(string key)
        {
            return _memcachedClient.Get(key);
        }

        public T GetValueFromCache<T>(string key)
        {
            return _memcachedClient.Get<T>(key);
        }

        public async Task<T> GetValueFromCacheAsync<T>(string key)
        {
            return await _memcachedClient.GetValueAsync<T>(key);
        }

        public IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}