using System;

namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// The exception that is thrown when an unknown error occures in the <see cref="T:MemcachedClient"/>
    /// </summary>
    //[global::System.Serializable]
    public class MemcachedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedException"/> class.
        /// </summary>
        public MemcachedException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedException"/> class with a specified error message.
        /// </summary>
        public MemcachedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MemcachedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public MemcachedException(string message, Exception inner) : base(message, inner) { }
    }
}