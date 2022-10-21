using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasyMemoryCache.Accessors
{
    public class ProtobufCacheAccessor : CacheAccessor
    {
        public ProtobufCacheAccessor(IDistributedCache cache) : base(cache)
        {
        }

        public override object Get(string key)
        {
            throw new NotImplementedException("Protobuf requires to know type in advance. Please use generic version of the method instead");
        }

        public override T Get<T>(string key)
        {
            var data = Cache.Get(key);
            if (data == null)
            {
                return default;
            }

            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public override async Task<T> GetAsync<T>(string key)
        {
            var data = await Cache.GetAsync(key);
            if (data == null)
            {
                return default;
            }

            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        protected override void SetInternal(string key, object value, DistributedCacheEntryOptions options)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                Cache.Set(key, stream.ToArray(), options);
            }
        }

        protected override async Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                await Cache.SetAsync(key, stream.ToArray(), options);
            }
        }
    }
}