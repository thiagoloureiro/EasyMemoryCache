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

        public SerializationType RedisSerialization { get; set; }
    }

    public enum CacheProvider
    {
        MemoryCache = 1,
        Redis = 2,
        Memcached = 3
    }

    public enum CacheTimeInterval
    {
        Seconds = 1,
        Minutes = 2,
        Hours = 3,
        Days = 4
    }
}