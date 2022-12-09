using EasyMemoryCache.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache.Memorycache
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
                    if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>))
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

                    if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>))
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
            _myCache.Clear();
        }

        public async Task InvalidateAllAsync()
        {
            await Task.Run(() =>
            {
                _myCache.Clear();
            });
        }

        public void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120)
        {
            _myCache.Set(key, value, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes));
        }

        public async Task SetValueToCacheAsync(string key, object value, int cacheTimeInMinutes = 120)
        {
            await Task.Run(() => _myCache.Set(key, value, DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes)));
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
            var keys = _myCache.GetKeys();
            var lst = new List<string>();
            foreach (var key in keys)
            {
                lst.Add(key.ToString());
            }
            return lst;
        }
    }
}