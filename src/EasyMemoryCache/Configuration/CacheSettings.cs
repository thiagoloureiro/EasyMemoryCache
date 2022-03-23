namespace EasyMemoryCache.Configuration
{
    public enum SerializationType
    {
        Newtonsoft,
        Protobuf
    }

    public class CacheSettings
    {
        public bool IsDistributed { get; set; } = false;
        public string RedisConnectionString { get; set; }
        public string RedisPassword { get; set; }
        public SerializationType RedisSerialization { get; set; }
    }
}
