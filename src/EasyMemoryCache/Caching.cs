using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EasyMemoryCache.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyMemoryCache
{
    public class Caching : ICaching, IDisposable
    {
        private readonly MemoryCache _myCache;
        private readonly NamedSemaphoreSlim _cacheLock = new NamedSemaphoreSlim(1);

        public Caching() : this(new MemoryCache(new MemoryCacheOptions()))
        {

        }

        [ActivatorUtilitiesConstructor]
        public Caching(IMemoryCache cache)
        {
            _myCache = (MemoryCache)cache;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = default;

            var data = _myCache.Get(cacheItemName);

            if (data != null)
                cachedObject = (T)data;

            if (data == null)
            {
                _cacheLock[cacheItemName].Wait();

                try
                {
                    cachedObject = objectSettingFunction();

                    var oType = cachedObject.GetType();
                    if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        if (((ICollection)cachedObject).Count > 0)
                        {
                            _myCache.Set(cacheItemName, cachedObject,
                                DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                        }
                        else if (cacheEmptyList)
                        {
                            _myCache.Set(cacheItemName, cachedObject,
                                DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                        }
                    }
                    else
                    {
                        _myCache.Set(cacheItemName, cachedObject, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                    }

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return cachedObject;
                }
                finally
                {
                    _cacheLock[cacheItemName].Release();
                }
                
            }
            return cachedObject;
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = default;
            var cacheObj = _myCache.Get(cacheItemName);

            if (cacheObj != null)
                cachedObject = (T)cacheObj;

            if (cacheObj == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                await _cacheLock[cacheItemName].WaitAsync().ConfigureAwait(false);
                try
                {
                    cachedObject = await objectSettingFunction().ConfigureAwait(false);

                    var oType = cachedObject.GetType();

                    if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        if (((ICollection)cachedObject).Count > 0)
                        {
                            _myCache.Set(cacheItemName, cachedObject,
                                DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                        }
                        else if (cacheEmptyList)
                        {
                            _myCache.Set(cacheItemName, cachedObject,
                                DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                        }
                    }
                    else
                    {
                        _myCache.Set(cacheItemName, cachedObject, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return cachedObject;
                }
                finally
                {
                    _cacheLock[cacheItemName].Release();
                }
            }
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

        public T GetValueFromCache<T>(string key)
        {
            return _myCache.Get<T>(key);
        }

        public void Dispose()
        {
            _myCache?.Dispose();
            _cacheLock?.Dispose();
        }

        public IEnumerable<string> GetKeys()
        {
            return _myCache.GetKeys<string>();
        }

        public IEnumerable<DataContainer> GetData()
        {
            var items = new List<DataContainer>();
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var collection = field.GetValue(_myCache) as ICollection;
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        var values = item.GetType().GetProperty("Value");
                        if (values != null)
                        {
                            var entry = (ICacheEntry)values.GetValue(item);
                            items.Add(new DataContainer(entry.Key.ToString(), entry.Size, entry.AbsoluteExpiration,
                                entry.Priority));
                        }
                    }
                }
            }

            return items;
        }
    }
}