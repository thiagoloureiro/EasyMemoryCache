using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EasyMemoryCache
{
    public class RedisCaching : ICaching
    {
        private readonly IDistributedCache _myCache;
        private readonly IServer _server;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1);

        public RedisCaching(IDistributedCache cache, IServer server)
        {
            _myCache = (RedisCache)cache;
            _server = server;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = default;

            var data = _myCache.GetString(cacheItemName);
            if (data != null)
            {
                cachedObject = JsonConvert.DeserializeObject<T>(data);
            }

            if (data == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                cachedObject = objectSettingFunction();
                var serializedObject = JsonConvert.SerializeObject(cachedObject);
                var entryOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
                };

                var oType = cachedObject.GetType();
                if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    if (((ICollection)cachedObject).Count > 0 || cacheEmptyList)
                    {
                        _myCache.SetString(cacheItemName, serializedObject, entryOptions);
                    }
                }
                else
                {
                    _myCache.SetString(cacheItemName, serializedObject, entryOptions);
                }
            }
            return cachedObject;
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = default;

            await _cacheLock.WaitAsync().ConfigureAwait(false);
            var data = await _myCache.GetStringAsync(cacheItemName);

            if (data != null)
            {
                cachedObject = JsonConvert.DeserializeObject<T>(data);
            }

            if (data == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                try
                {
                    cachedObject = await objectSettingFunction().ConfigureAwait(false);
                    var serializedObject = JsonConvert.SerializeObject(cachedObject);
                    var entryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
                    };

                    var oType = cachedObject.GetType();

                    if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        if (((ICollection)cachedObject).Count > 0 || cacheEmptyList)
                        {
                            await _myCache.SetStringAsync(cacheItemName, serializedObject, entryOptions);
                        }
                    }
                    else
                    {
                        await _myCache.SetStringAsync(cacheItemName, serializedObject, entryOptions);
                    }
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
            _server.FlushDatabase();
        }

        public void SetValueToCache(string key, object value, int cacheTimeInMinutes = 120)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            var entryOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
            };

            _myCache.SetString(key, serializedObject, entryOptions);
        }

        public object GetValueFromCache(string key)
        {
            return _myCache.Get(key);
        }

        public void Dispose()
        {
            if(_myCache is RedisCache cache)
            {
                cache.Dispose();
            }
        }

        public IEnumerable<string> GetKeys()
        {
            return _server.Keys().Select(x => x.ToString());
        }

        public IEnumerable<DataContainer> GetData()
        {
            throw new NotImplementedException();
        }
    }
}
