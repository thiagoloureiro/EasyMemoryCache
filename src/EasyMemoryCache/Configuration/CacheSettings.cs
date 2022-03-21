namespace EasyMemoryCache.Configuration
{
    public class CacheSettings
    {
        public bool IsDistributed { get; set; } = false;
        public string RedisConnectionString { get; set; }
        public string RedisPassword { get; set; }
    }
}
