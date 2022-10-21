namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// Inidicates the mode how the items are stored in Memcached.
    /// </summary>
    public enum StoreMode
    {
        /// <summary>
        /// Store the data, but only if the server does not already hold data for a given key
        /// </summary>
        Add = 1,

        /// <summary>
        /// Store the data, but only if the server does already hold data for a given key
        /// </summary>
        Replace,

        /// <summary>
        /// Store the data, overwrite if already exist
        /// </summary>
        Set
    };

    internal enum StoreCommand
    {
        /// <summary>
        /// Store the data, but only if the server does not already hold data for a given key
        /// </summary>
        Add = 1,

        /// <summary>
        /// Store the data, but only if the server does already hold data for a given key
        /// </summary>
        Replace,

        /// <summary>
        /// Store the data, overwrite if already exist
        /// </summary>
        Set,

        /// <summary>
        /// Appends the data to an existing key's data
        /// </summary>
        Append,

        /// <summary>
        /// Inserts the data before an existing key's data
        /// </summary>
        Prepend,

        /// <summary>
        /// Stores the data only if it has not been updated by someone else. Uses a "transaction id" to check for modification.
        /// </summary>
        CheckAndSet
    };
}