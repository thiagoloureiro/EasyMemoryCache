using System.Collections.Generic;
using System.Net;
using EasyMemoryCache.Memcached.Memcached;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Configuration
{
    /// <summary>
    /// Defines an interface for configuring the <see cref="T:MemcachedClient"/>.
    /// </summary>
    public interface IMemcachedClientConfiguration
    {
        /// <summary>
        /// Gets a list of <see cref="T:IPEndPoint"/> each representing a Memcached server in the pool.
        /// </summary>
        IList<EndPoint> Servers { get; }

        /// <summary>
        /// Gets the configuration of the socket pool.
        /// </summary>
        ISocketPoolConfiguration SocketPool { get; }

        /// <summary>
        /// Gets the authentication settings.
        /// </summary>
        IAuthenticationConfiguration Authentication { get; }

        /// <summary>
        /// Creates an <see cref="T:EasyMemoryCache.Memcached.Memcached.IMemcachedKeyTransformer"/> instance which will be used to convert item keys for Memcached.
        /// </summary>
        IMemcachedKeyTransformer CreateKeyTransformer();

        /// <summary>
        /// Creates an <see cref="T:EasyMemoryCache.Memcached.Memcached.IMemcachedNodeLocator"/> instance which will be used to assign items to Memcached nodes.
        /// </summary>
        IMemcachedNodeLocator CreateNodeLocator();

        /// <summary>
        /// Creates an <see cref="T:EasyMemoryCache.Memcached.Memcached.Transcoders.ITranscoder"/> instance which will be used to serialize or deserialize items.
        /// </summary>
        ITranscoder CreateTranscoder();

        IServerPool CreatePool();

        bool UseSslStream { get; }
    }
}