using System;
using System.Collections.Generic;

namespace EasyMemoryCache.Memcached.Configuration
{
    /// <summary>
    /// Defines an interface for configuring the authentication paramaters the <see cref="T:MemcachedClient"/>.
    /// </summary>
    public interface IAuthenticationConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the <see cref="T:Enyim.Caching.Memcached.IAuthenticationProvider"/> which will be used authehticate the connections to the Memcached nodes.
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// Gets or sets the parameters passed to the authenticator instance.
        /// </summary>
        Dictionary<string, object> Parameters { get; }
    }
}