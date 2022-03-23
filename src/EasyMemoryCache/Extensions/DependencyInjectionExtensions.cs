using System;
using EasyMemoryCache;
using EasyMemoryCache.Accessors;
using EasyMemoryCache.Configuration;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddEasyCache(this IServiceCollection services, CacheSettings settings)
        {
            return settings.IsDistributed
                ? services.AddEasyRedisCache(settings)
                : services.AddEasyMemoryCache();
        }

        private static IServiceCollection AddEasyRedisCache(this IServiceCollection services, CacheSettings settings)
        {
            var configurationOptions = new ConfigurationOptions()
            {
                AllowAdmin = true,
                Ssl = false,
                Password = settings.RedisPassword
            };
            configurationOptions.EndPoints.Add(settings.RedisConnectionString);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = configurationOptions;
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
                var connection = ConnectionMultiplexer.Connect(configurationOptions);
                return connection.GetServer(settings.RedisConnectionString);
            });

            return services;
        }

        private static IServiceCollection AddEasyMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICaching, EasyMemoryCache.Caching>();

            return services;
        }
    }
}
