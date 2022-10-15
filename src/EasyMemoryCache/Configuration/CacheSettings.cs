namespace EasyMemoryCache.Configuration
{
    public enum SerializationType
    {
        Newtonsoft,
        Protobuf
    }

    public class CacheSettings
    {
        public CacheProvider CacheProvider { get; set; } = CacheProvider.MemoryCache;
        public string RedisConnectionString { get; set; }
        public string RedisPassword { get; set; }
        public SerializationType RedisSerialization { get; set; }
    }

    public enum CacheProvider
    {
        MemoryCache = 1,
        Redis = 2,
        Memcached = 3
    }
}