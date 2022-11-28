using EasyMemoryCache;
using EasyMemoryCache.Accessors;
using EasyMemoryCache.Configuration;
using EasyMemoryCache.Memcached;
using EasyMemoryCache.Redis;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddEasyCache(this IServiceCollection services, CacheSettings settings, IConfigurationSection section = null)
        {
            switch (settings.CacheProvider)
            {
                case CacheProvider.MemoryCache:
                    return services.AddEasyMemoryCache();

                case CacheProvider.Redis:
                    return services.AddEasyRedisCache(settings);

                case CacheProvider.Memcached:
                    return services.AddEasyMemcachedCache(section);

                default:
                    return services.AddEasyMemoryCache();
            }
        }

        private static IServiceCollection AddEasyMemcachedCache(this IServiceCollection services, IConfigurationSection section)
        {
            services.AddMemcached(section);
            services.AddSingleton<ICaching, MemcachedCaching>();

            return services;
        }

        private static IServiceCollection AddEasyRedisCache(this IServiceCollection services, CacheSettings settings)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    settings.RedisConnectionString;
            });

            switch (settings.RedisSerialization)
            {
                case SerializationType.Protobuf:
                    services.AddSingleton<CacheAccessor, ProtobufCacheAccessor>();
                    break;

                default:
                    services.AddSingleton<CacheAccessor, NewtonsoftCacheAccessor>();
                    break;
            }

            services.AddSingleton<ICaching, RedisCaching>();
            services.AddSingleton(sp =>
            {
                var host = settings.RedisConnectionString.Split(',')[0];
                var connection = ConnectionMultiplexer.Connect(settings.RedisConnectionString);
                return connection.GetServer(host);
            });

            return services;
        }

        private static IServiceCollection AddEasyMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICaching, EasyMemoryCache.Memorycache.Caching>();

            return services;
        }
    }
}