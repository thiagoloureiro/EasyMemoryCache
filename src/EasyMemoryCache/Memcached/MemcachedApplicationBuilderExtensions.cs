using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached
{
    public static class MemcachedApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMemcached(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<IMemcachedClient>>();
            try
            {
                var client = app.ApplicationServices.GetRequiredService<IMemcachedClient>();
                client.GetValueAsync<string>("CacheSettings").Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to UseMemcached");
            }

            return app;
        }
    }
}