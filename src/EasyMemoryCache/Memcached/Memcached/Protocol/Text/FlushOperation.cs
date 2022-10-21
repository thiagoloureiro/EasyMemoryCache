using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class FlushOperation : Operation, IFlushOperation
    {
        public FlushOperation()
        { }

        protected internal override IList<System.ArraySegment<byte>> GetBuffer()
        {
            return TextSocketHelper.GetCommandBuffer("flush_all" + TextSocketHelper.CommandTerminator);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            TextSocketHelper.ReadResponse(socket);
            return new TextOperationResult().Pass();
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            TextSocketHelper.ReadResponse(socket);
            return new ValueTask<IOperationResult>(new TextOperationResult().Pass());
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}