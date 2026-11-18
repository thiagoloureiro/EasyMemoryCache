using Microsoft.Extensions.Caching.Distributed;
using ProtoBuf;
using System;
using System.IO;
using System.Threading;
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

        public override async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await Cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            if (data == null)
            {
                return default;
            }

            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public override bool TryGet<T>(string key, out T value)
        {
            var data = Cache.Get(key);
            if (data == null)
            {
                value = default;
                return false;
            }

            using (var stream = new MemoryStream(data))
            {
                value = Serializer.Deserialize<T>(stream);
                return true;
            }
        }

        public override async Task<CacheResult<T>> TryGetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var data = await Cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
            if (data == null)
                return CacheResult<T>.Miss;

            using (var stream = new MemoryStream(data))
            {
                return CacheResult<T>.Hit(Serializer.Deserialize<T>(stream));
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

        protected override async Task SetInternalAsync(string key, object value, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                await Cache.SetAsync(key, stream.ToArray(), options, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}