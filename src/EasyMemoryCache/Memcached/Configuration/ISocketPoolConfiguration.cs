using System;
using EasyMemoryCache.Memcached.Memcached.FailurePolicy;

namespace EasyMemoryCache.Memcached.Configuration
{
    /// <summary>
    /// Defines an interface for configuring the socket pool for the <see cref="T:MemcachedClient"/>.
    /// </summary>
    public interface ISocketPoolConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating the minimum amount of sockets per server in the socket pool.
        /// </summary>
        /// <returns>The minimum amount of sockets per server in the socket pool.</returns>
        int MinPoolSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the maximum amount of sockets per server in the socket pool.
        /// </summary>
        /// <returns>The maximum amount of sockets per server in the socket pool.</returns>
        int MaxPoolSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that specifies the amount of time after which the connection attempt will fail.
        /// </summary>
        /// <returns>The value of the connection timeout.</returns>
        TimeSpan ConnectionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that specifies the amount of time after which the getting a connection from the pool will fail.
        /// </summary>
        /// <returns>The value of the queue timeout.</returns>
        TimeSpan QueueTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that specifies the amount of time after which receiving data from the socket will fail.
        /// </summary>
        /// <returns>The value of the receive timeout.</returns>
        TimeSpan ReceiveTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that specifies the amount of time after which an unresponsive (dead) server will be checked if it is working.
        /// </summary>
        /// <returns>The value of the dead timeout.</returns>
        TimeSpan DeadTimeout
        {
            get;
            set;
        }

        TimeSpan InitPoolTimeout { get; set; }

        INodeFailurePolicyFactory FailurePolicyFactory { get; set; }
    }
}