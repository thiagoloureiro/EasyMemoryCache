using EasyMemoryCache.Accessors;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyMemoryCache.Redis
{
    public class RedisCaching : ICaching, IDisposable
    {
        private readonly CacheAccessor _cacheAccessor;
        private readonly IServer _server;
        private readonly NamedSemaphoreSlim _cacheLock = new NamedSemaphoreSlim(1);

        public RedisCaching(CacheAccessor cacheAccessor, IServer server)
        {
            _cacheAccessor = cacheAccessor;
            _server = server;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = _cacheAccessor.Get<T>(cacheItemName);

            if (cachedObject == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                _cacheLock[cacheItemName].Wait();
                try
                {
                    cachedObject = objectSettingFunction();
                    var oType = cachedObject.GetType();
                    if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (((ICollection)cachedObject).Count > 0 || cacheEmptyList)
                        {
                            _cacheAccessor.Set(cacheItemName, cachedObject, cacheTimeInMinutes);
                        }
                    }
                    else
                    {
                        _cacheAccessor.Set(cacheItemName, cachedObject, cacheTimeInMinutes);
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
            T cachedObject = await _cacheAccessor.GetAsync<T>(cacheItemName).ConfigureAwait(false);

            if (cachedObject == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                await _cacheLock[cacheItemName].WaitAsync().ConfigureAwait(false);
                try
                {
                    cachedObject = await objectSettingFunction().ConfigureAwait(false);

                    var oType = cachedObject.GetType();
                    if (oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        if (((ICollection)cachedObject).Count > 0 || cacheEmptyList)
                        {
                            await _cacheAccessor.SetAsync(cacheItemName, cachedObject, cacheTimeInMinutes).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await _cacheAccessor.SetAsync(cacheItemName, cachedObject, cacheTimeInMinutes).ConfigureAwait(false);
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
            _cacheAccessor.Remove(key);
        }

        public void InvalidateAll()
        {
            _server.FlushDatabase();
        }

        public async Task InvalidateAllAsync()
        {
            await _server.FlushDatabaseAsync();
        }

        public void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120)
        {
            _cacheAccessor.Set(key, value, cacheTimeInMinutes);
        }

        public async Task SetValueToCacheAsync(string key, object value, int cacheTimeInMinutes = 120)
        {
            await _cacheAccessor.SetAsync(key, value, cacheTimeInMinutes);
        }

        public object GetValueFromCache(string key)
        {
            return _cacheAccessor.Get(key);
        }

        public T GetValueFromCache<T>(string key)
        {
            return _cacheAccessor.Get<T>(key);
        }

        public void Dispose()
        {
            _cacheAccessor.Dispose();
            _cacheLock.Dispose();
        }

        public IEnumerable<string> GetKeys()
        {
            return _server.Keys().Select(x => x.ToString());
        }
    }
}