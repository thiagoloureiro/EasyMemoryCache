using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EasyMemoryCache
{
    public class RedisCaching : ICaching, IDisposable
    {
        private readonly IDistributedCache _myCache;
        private readonly IServer _server;
        private readonly NamedSemaphoreSlim _cacheLock = new NamedSemaphoreSlim(1);

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
                _cacheLock[cacheItemName].Wait();
                try
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

            var data = await _myCache.GetStringAsync(cacheItemName);
            if (data != null)
            {
                cachedObject = JsonConvert.DeserializeObject<T>(data);
            }

            if (data == null || EqualityComparer<T>.Default.Equals(cachedObject, default))
            {
                await _cacheLock[cacheItemName].WaitAsync().ConfigureAwait(false);
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
                            await _myCache.SetStringAsync(cacheItemName, serializedObject, entryOptions).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await _myCache.SetStringAsync(cacheItemName, serializedObject, entryOptions).ConfigureAwait(false);
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
            var data = _myCache.GetString(key);
            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject(data);
        }

        public void Dispose()
        {
            if(_myCache is RedisCache cache)
            {
                cache.Dispose();
            }
            _cacheLock.Dispose();
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
