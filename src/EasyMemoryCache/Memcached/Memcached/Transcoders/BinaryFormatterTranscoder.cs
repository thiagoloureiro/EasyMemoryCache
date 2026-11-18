using System;

namespace EasyMemoryCache.Memcached.Memcached.Transcoders
{
    public class BinaryFormatterTranscoder : DefaultTranscoder
    {
        protected override ArraySegment<byte> SerializeObject(object value)
        {
            throw NotSupported();
        }

        public override T Deserialize<T>(CacheItem item)
        {
            throw NotSupported();
        }

        protected override object DeserializeObject(ArraySegment<byte> value)
        {
            throw NotSupported();
        }

        private static NotSupportedException NotSupported()
        {
            return new NotSupportedException("BinaryFormatterTranscoder cannot be used. Deserializing BinaryFormatter payloads from cache can execute code.");
        }
    }
}