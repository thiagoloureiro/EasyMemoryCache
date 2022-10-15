using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached
{
    public static class EnyimMemcachedApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseEnyimMemcached(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILogger<IMemcachedClient>>();
            try
            {
                var client = app.ApplicationServices.GetRequiredService<IMemcachedClient>();
                client.GetValueAsync<string>("EnyimMemcached").Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to UseEnyimMemcached");
            }

            return app;
        }
    }
}
