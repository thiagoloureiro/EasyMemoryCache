namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// Converts Memcached item keys into a custom format.
    /// </summary>
    public interface IMemcachedKeyTransformer
    {
        /// <summary>
        /// Performs the transformation.
        /// </summary>
        /// <param name="key">The key to be transformed.</param>
        /// <returns>the transformed key.</returns>
        string Transform(string key);
    }
}