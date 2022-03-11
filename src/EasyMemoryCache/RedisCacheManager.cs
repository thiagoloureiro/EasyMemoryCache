using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyMemoryCache
{
    public class RedisCacheManager : ICaching
    {
        private readonly IDatabase _cache;
        private readonly IServer _server;
        private readonly string _redisConnString;
        private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1);

        private readonly Lazy<ConnectionMultiplexer> LazyRedisConnection = new Lazy<ConnectionMultiplexer>(() =>
                                                                                                                              {
                                                                                                                                  string cacheConnection = "localhost,allowAdmin=true";
                                                                                                                                  return ConnectionMultiplexer.Connect(cacheConnection);
                                                                                                                              });

        public RedisCacheManager()
        {
            _cache = LazyRedisConnection.Value.GetDatabase();
            _server = LazyRedisConnection.Value.GetServer("localhost", 6379);
        }

        public bool CheckIfKeyExists(string key)
        {
            return _cache.KeyExists(key);
        }

        public async Task<bool> CheckIfKeyExistsAsync(string key)
        {
            return await _cache.KeyExistsAsync(key).ConfigureAwait(false);
        }

        public List<T> GetAll<T>(string key)
        {
            if (CheckIfKeyExists(key))
                return JsonConvert.DeserializeObject<List<T>>(_cache.StringGet(key));
            else
                return default;
        }

        public async Task<List<T>> GetAllAsync<T>(string key)
        {
            if (await CheckIfKeyExistsAsync(key).ConfigureAwait(false))
                return JsonConvert.DeserializeObject<List<T>>(await _cache.StringGetAsync(key).ConfigureAwait(false));
            else
                return default;
        }

        public IEnumerable<DataContainer> GetData()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeys()
        {
            var lstKeys = new List<string>();
            var keys = _server.Keys(_cache.Database);

            foreach (var key in keys)
            {
                lstKeys.Add(key.ToString());
            }
            return lstKeys;
        }

        public T GetOrSetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction, bool cacheEmptyList = false)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetOrSetObjectFromCacheAsync<T>(string cacheItemName, int cacheTimeInMinutes, Func<Task<T>> objectSettingFunction, bool cacheEmptyList = false)
        {
            T cachedObject = default;

            await _cacheLock.WaitAsync().ConfigureAwait(false);
            var cacheObj = await _cache.StringGetAsync(cacheItemName);

            if (cacheObj.IsNull)
            {
                try
                {
                    cachedObject = await objectSettingFunction().ConfigureAwait(false);

                    var oType = cachedObject.GetType();

                    _cache.StringSet(cacheItemName, JsonConvert.SerializeObject(cachedObject), TimeSpan.FromMinutes(cacheTimeInMinutes));
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

        public T GetValueFromCache<T>(string key)
        {
            if (CheckIfKeyExists(key))
                return JsonConvert.DeserializeObject<T>(_cache.StringGet(key));
            else
                return default;
        }

        public void Invalidate(string key)
        {
            _cache.KeyDelete(key);
        }

        public async Task InvalidateAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public void InvalidateAll()
        {
            _server.FlushAllDatabases();
        }

        public async Task InvalidateAllAsync()
        {
            await _server.FlushAllDatabasesAsync();
        }

        public void Remove(string key)
        {
            _cache.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public void SetValueToCache<T>(string key, T value, int cacheTimeInMinutes = 120)
        {
            if (CheckIfKeyExists(key))
                Remove(key);
            _cache.StringSet(key, JsonConvert.SerializeObject(value));
            _cache.KeyExpire(key, TimeSpan.FromMinutes(cacheTimeInMinutes));
        }

        public void SetValuesToCache<T>(string key, List<T> value, int cacheTimeInMinutes = 120)
        {
            if (CheckIfKeyExists(key))
                Remove(key);
            _cache.StringSet(key, JsonConvert.SerializeObject(value));
            _cache.KeyExpire(key, TimeSpan.FromMinutes(cacheTimeInMinutes));
        }
    }
}