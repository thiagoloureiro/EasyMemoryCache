namespace EasyMemoryCache.Memcached.Memcached.Protocol
{
    /// <summary>
    /// Base class for implementing operations working with keyed items.
    /// </summary>
    public abstract class SingleItemOperation : Operation, ISingleItemOperation
    {
        protected SingleItemOperation(string key)
        {
            this.Key = key;
        }

        public string Key { get; private set; }

        public ulong Cas { get; set; }

        /// <summary>
        /// The item key of the current operation.
        /// </summary>
        string ISingleItemOperation.Key
        {
            get { return this.Key; }
        }

        ulong ISingleItemOperation.CasValue
        {
            get { return this.Cas; }
        }
    }
}