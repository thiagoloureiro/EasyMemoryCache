using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EasyMemoryCache.Memcached.Memcached.Transcoders
{
    public class BinaryFormatterTranscoder : DefaultTranscoder
    {
        protected override ArraySegment<byte> SerializeObject(object value)
        {
            using (var ms = new MemoryStream())
            {
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(ms, value);
                return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
#pragma warning restore SYSLIB0011
            }
        }

        public override T Deserialize<T>(CacheItem item)
        {
            return (T)base.Deserialize(item);
        }

        protected override object DeserializeObject(ArraySegment<byte> value)
        {
            using (var ms = new MemoryStream(value.Array, value.Offset, value.Count))
            {
#pragma warning disable SYSLIB0011
                return new BinaryFormatter().Deserialize(ms);
#pragma warning restore SYSLIB0011
            }
        }
    }
}