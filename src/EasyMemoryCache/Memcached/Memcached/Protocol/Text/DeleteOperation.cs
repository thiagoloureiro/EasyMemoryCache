using System;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class DeleteOperation : SingleItemOperation, IDeleteOperation
    {
        internal DeleteOperation(string key) : base(key)
        {
        }

        protected internal override System.Collections.Generic.IList<ArraySegment<byte>> GetBuffer()
        {
            var command = "delete " + this.Key + TextSocketHelper.CommandTerminator;

            return TextSocketHelper.GetCommandBuffer(command);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            return new TextOperationResult
            {
                Success = String.Compare(TextSocketHelper.ReadResponse(socket), "DELETED", StringComparison.Ordinal) == 0
            };
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            return new ValueTask<IOperationResult>(
                new TextOperationResult
                {
                    Success = String.Compare(TextSocketHelper.ReadResponse(socket), "DELETED", StringComparison.Ordinal) == 0
                });
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}