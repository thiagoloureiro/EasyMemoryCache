using System;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class ConcateOperation : StoreOperationBase, IConcatOperation
    {
        private readonly ConcatenationMode mode;

        internal ConcateOperation(ConcatenationMode mode, string key, ArraySegment<byte> data)
            : base(mode == ConcatenationMode.Append
                    ? StoreCommand.Append
                    : StoreCommand.Prepend, key, new CacheItem() { Data = data, Flags = 0 }, 0, 0)
        {
            this.mode = mode;
        }

        ConcatenationMode IConcatOperation.Mode
        {
            get { return this.mode; }
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}