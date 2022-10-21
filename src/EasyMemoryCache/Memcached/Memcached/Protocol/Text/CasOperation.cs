using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class CasOperation : StoreOperationBase, IStoreOperation
    {
        internal CasOperation(string key, CacheItem value, uint expires, ulong cas)
            : base(StoreCommand.CheckAndSet, key, value, expires, cas) { }

        StoreMode IStoreOperation.Mode
        {
            get { return StoreMode.Set; }
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}