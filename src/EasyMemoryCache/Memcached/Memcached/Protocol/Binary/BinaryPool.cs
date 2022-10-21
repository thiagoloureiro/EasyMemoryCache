using System;
using System.Net;
using EasyMemoryCache.Memcached.Configuration;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// Server pool implementing the binary protocol.
    /// </summary>
    public class BinaryPool : DefaultServerPool
    {
        private readonly ISaslAuthenticationProvider authenticationProvider;
        private readonly IMemcachedClientConfiguration configuration;
        private readonly ILogger _logger;

        public BinaryPool(IMemcachedClientConfiguration configuration, ILogger logger)
            : base(configuration, new BinaryOperationFactory(logger), logger)
        {
            this.authenticationProvider = GetProvider(configuration);
            this.configuration = configuration;
            _logger = logger;
        }

        protected override IMemcachedNode CreateNode(EndPoint endpoint)
        {
            return new BinaryNode(endpoint, this.configuration.SocketPool, this.authenticationProvider, _logger, this.configuration.UseSslStream);
        }

        private static ISaslAuthenticationProvider GetProvider(IMemcachedClientConfiguration configuration)
        {
            // create&initialize the authenticator, if any
            // we'll use this single instance everywhere, so it must be thread safe
            IAuthenticationConfiguration auth = configuration.Authentication;
            if (auth != null)
            {
                Type t = auth.Type;
                var provider = (t == null) ? null : FastActivator.Create(t) as ISaslAuthenticationProvider;

                if (provider != null)
                {
                    provider.Initialize(auth.Parameters);
                    return provider;
                }
            }

            return null;
        }
    }
}