﻿using System;
using EasyMemoryCache.Memcached.Configuration;
using EasyMemoryCache.Memcached.Memcached;
using EasyMemoryCache.Memcached.Memcached.KeyTransformers;
using EasyMemoryCache.Memcached.Memcached.Transcoders;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyMemoryCache.Memcached
{
    public static class MemcachedServiceCollectionExtensions
    {
        /// <summary>
        /// Add EnyimMemcached to the specified <see cref="IServiceCollection"/>.
        /// Read configuration via IConfiguration.GetSection("Memcached")
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMemcached(this IServiceCollection services)
        {
            return AddMemcachedInternal(services, null);
        }

        public static IServiceCollection AddMemcached(this IServiceCollection services, Action<MemcachedClientOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            return AddMemcachedInternal(services, s => s.Configure(setupAction));
        }

        public static IServiceCollection AddMemcached(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            if (!configurationSection.Exists())
            {
                throw new ArgumentNullException($"{configurationSection.Key} in appsettings.json");
            }

            return AddMemcachedInternal(services, s => s.Configure<MemcachedClientOptions>(configurationSection));
        }

        public static IServiceCollection AddMemcached(this IServiceCollection services, IConfiguration configuration, string sectionKey = "CacheSettings")
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var section = configuration.GetSection(sectionKey);
            if (!section.Exists())
            {
                throw new ArgumentNullException($"{sectionKey} in appsettings.json");
            }

            return AddMemcachedInternal(services, s => s.Configure<MemcachedClientOptions>(section));
        }

        private static IServiceCollection AddMemcachedInternal(IServiceCollection services, Action<IServiceCollection> configure)
        {
            services.AddOptions();
            configure?.Invoke(services);

            services.TryAddSingleton<ITranscoder, DefaultTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();
            services.TryAddSingleton<IMemcachedClientConfiguration, MemcachedClientConfiguration>();
            services.AddSingleton<MemcachedClient>();

            services.AddSingleton<IMemcachedClient>(factory => factory.GetService<MemcachedClient>());
            services.AddSingleton<IDistributedCache>(factory => factory.GetService<MemcachedClient>());

            return services;
        }

        public static IServiceCollection AddMemcached<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionKey)
        {
            services.AddOptions();
            services.Configure<MemcachedClientOptions>(sectionKey, configuration.GetSection(sectionKey));
            services.TryAddSingleton<ITranscoder, DefaultTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();

            services.AddSingleton<IMemcachedClient<T>>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var options = sp.GetRequiredService<IOptionsMonitor<MemcachedClientOptions>>();
                var conf = new MemcachedClientConfiguration(loggerFactory, options.Get(sectionKey));
                return new MemcachedClient<T>(loggerFactory, conf);
            });

            return services;
        }
    }
}