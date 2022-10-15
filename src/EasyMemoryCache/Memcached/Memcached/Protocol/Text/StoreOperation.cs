using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class StoreOperation : StoreOperationBase, IStoreOperation
    {
        private StoreMode mode;

        internal StoreOperation(StoreMode mode, string key, CacheItem value, uint expires)
            : base((StoreCommand)mode, key, value, expires, 0)
        {
            this.mode = mode;
        }

        StoreMode IStoreOperation.Mode
        {
            get { return this.mode; }
        }
    }
}