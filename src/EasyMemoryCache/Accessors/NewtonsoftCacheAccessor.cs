using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EasyMemoryCache.Accessors
{
    public class NewtonsoftCacheAccessor : CacheAccessor
    {
        public NewtonsoftCacheAccessor(IDistributedCache cache) : base(cache)
        {
        }

        public override object Get(string key)
        {
            var data = Cache.GetString(key);
            if (data == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(data);
        }

        public override T Get<T>(string key)
        {
            var data = Cache.GetString(key);
            if (data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            var data = await Cache.GetStringAsync(key);
            if (data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        protected override void SetInternal(string key, object value, DistributedCacheEntryOptions options)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            Cache.SetString(key, serializedObject, options);
        }

        protected override async Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            await Cache.SetStringAsync(key, serializedObject, options);
        }
    }
}