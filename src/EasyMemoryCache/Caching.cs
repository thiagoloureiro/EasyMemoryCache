using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache
{
    public class Caching : ICaching, IDisposable
    {
        private readonly MemoryCache _myCache = new MemoryCache(new MemoryCacheOptions());
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1);
        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction)
        {
            lock (objectSettingFunction)
            {
                
            }
            T cachedObject = default;

            var cacheObj = _myCache.Get(cacheItemName);

            if (cacheObj != null)
                cachedObject = (T)cacheObj;

            if (cacheObj == null)
            {
                cachedObject = objectSettingFunction();
                _myCache.Set(cacheItemName, cachedObject, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
            }
            return cachedObject;
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction)
        {
            T cachedObject = default;

            await _cacheLock.WaitAsync();
            var cacheObj = _myCache.Get(cacheItemName);

            if (cacheObj != null)
            {
                cachedObject = (T)cacheObj;
                if (cachedObject is DateTime)
                {
                    if ((DateTime)cacheObj == DateTime.MinValue)
                    {
                        try
                        {
                            cachedObject = await objectSettingFunction();
                            _myCache.Set(cacheItemName, cachedObject,
                                DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine(err.Message);
                            _cacheLock.Release();
                            return cachedObject;
                        }
                    }
                }
            }

            if (cacheObj == null)
            {
                try
                {
                    cachedObject = await objectSettingFunction();
                    _myCache.Set(cacheItemName, cachedObject, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    _cacheLock.Release();
                    return cachedObject;
                }
            }
            _cacheLock.Release();
            return cachedObject;
        }

        public void Invalidate(string key)
        {
            _myCache.Remove(key);
        }

        public void InvalidateAll()
        {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var collection = field.GetValue(_myCache) as ICollection;
                var items = new List<string>();
                if (collection != null)
                    foreach (var item in collection)
                    {
                        var methodInfo = item.GetType().GetProperty("Key");
                        if (methodInfo != null)
                        {
                            var val = methodInfo.GetValue(item);
                            items.Add(val.ToString());
                        }
                    }

                foreach (var item in items)
                {
                    _myCache.Remove(item);
                }
            }
        }

        public void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120)
        {
            _myCache.Set(key, value, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
        }

        public object GetValueFromCache(string key)
        {
            return _myCache.Get(key);
        }

        public void Dispose()
        {
            _myCache?.Dispose();
        }
    }
}