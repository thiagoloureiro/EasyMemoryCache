namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// Represent a stat item returned by Memcached.
    /// </summary>
    public enum StatItem : int
    {
        /// <summary>
        /// The number of seconds the server has been running.
        /// </summary>
        Uptime = 0,

        /// <summary>
        /// Current time according to the server.
        /// </summary>
        ServerTime,

        /// <summary>
        /// The version of the server.
        /// </summary>
        Version,

        /// <summary>
        /// The number of items stored by the server.
        /// </summary>
        ItemCount,

        /// <summary>
        /// The total number of items stored by the server including the ones whihc have been already evicted.
        /// </summary>
        TotalItems,

        /// <summary>
        /// Number of active connections to the server.
        /// </summary>
        ConnectionCount,

        /// <summary>
        /// The total number of connections ever made to the server.
        /// </summary>
        TotalConnections,

        /// <summary>
        /// ?
        /// </summary>
        ConnectionStructures,

        /// <summary>
        /// Number of get operations performed by the server.
        /// </summary>
        GetCount,

        /// <summary>
        /// Number of set operations performed by the server.
        /// </summary>
        SetCount,

        /// <summary>
        /// Cache hit.
        /// </summary>
        GetHits,

        /// <summary>
        /// Cache miss.
        /// </summary>
        GetMisses,

        /// <summary>
        /// ?
        /// </summary>
        UsedBytes,

        /// <summary>
        /// Number of bytes read from the server.
        /// </summary>
        BytesRead,

        /// <summary>
        /// Number of bytes written to the server.
        /// </summary>
        BytesWritten,

        /// <summary>
        /// ?
        /// </summary>
        MaxBytes
    }
}