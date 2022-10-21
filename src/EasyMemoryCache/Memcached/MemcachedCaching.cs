using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache.Memcached
{
    public class MemcachedCaching : ICaching
    {
        private readonly IMemcachedClient _memcachedClient;

        public MemcachedCaching(IMemcachedClient memcachedClient)
        {
            _memcachedClient = memcachedClient;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction,
            bool cacheEmptyList = false)
        {
            T result;

            var data = _memcachedClient.Get(cacheItemName);
            if (data == null)
            {
                result = objectSettingFunction();
                _memcachedClient.Set(cacheItemName, result, cacheTimeInMinutes / 60);
            }
            else
            {
                return (T)data;
            }
            return result;
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction,
            bool cacheEmptyList = false)
        {
            return await _memcachedClient.GetValueOrCreateAsync(cacheItemName, cacheTimeInMinutes / 60,
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

        public void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120)
        {
            _memcachedClient.Add(key, value, cacheTimeInMinutes / 60);
        }

        public async Task SetValueToCacheAsync(string key, object value, int cacheTimeInMinutes = 120)
        {
            await _memcachedClient.AddAsync(key, value, cacheTimeInMinutes / 60).ConfigureAwait(false);
        }

        public object GetValueFromCache(string key)
        {
            return _memcachedClient.Get(key);
        }

        public T GetValueFromCache<T>(string key)
        {
            return _memcachedClient.Get<T>(key);
        }

        public IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataContainer> GetData()
        {
            throw new NotImplementedException();
        }
    }
}