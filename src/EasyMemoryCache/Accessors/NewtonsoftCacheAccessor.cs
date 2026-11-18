using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading;
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

        public override async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await Cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
            if (data == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(data);
        }

        public override bool TryGet<T>(string key, out T value)
        {
            var data = Cache.GetString(key);
            if (data == null)
            {
                value = default;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(data);
            return true;
        }

        public override async Task<CacheResult<T>> TryGetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await Cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
            if (data == null)
                return CacheResult<T>.Miss;

            return CacheResult<T>.Hit(JsonConvert.DeserializeObject<T>(data));
        }

        protected override void SetInternal(string key, object value, DistributedCacheEntryOptions options)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            Cache.SetString(key, serializedObject, options);
        }

        protected override async Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            var serializedObject = JsonConvert.SerializeObject(value);
            await Cache.SetStringAsync(key, serializedObject, options, cancellationToken).ConfigureAwait(false);
        }
    }
}