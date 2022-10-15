using System;

namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// The exception that is thrown when a client error occures during communicating with the Memcached servers.
    /// </summary>
    //[global::System.Serializable]
    public class MemcachedClientException : MemcachedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedClientException"/> class.
        /// </summary>
        public MemcachedClientException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedClientException"/> class with a specified error message.
        /// </summary>
        public MemcachedClientException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedClientException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public MemcachedClientException(string message, Exception inner) : base(message, inner) { }
    }
}