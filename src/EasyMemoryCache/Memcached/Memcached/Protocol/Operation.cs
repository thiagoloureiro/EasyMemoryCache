using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;

namespace EasyMemoryCache.Memcached.Memcached.Protocol
{
    /// <summary>
    /// Base class for implementing operations.
    /// </summary>
    public abstract class Operation : IOperation
    {
        protected Operation()
        { }

        protected internal abstract IList<ArraySegment<byte>> GetBuffer();

        protected internal abstract IOperationResult ReadResponse(PooledSocket socket);

        protected internal abstract ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket);

        protected internal abstract Task<bool> ReadResponseAsync(PooledSocket socket, Action<bool> next);

        IList<ArraySegment<byte>> IOperation.GetBuffer()
        {
            return this.GetBuffer();
        }

        IOperationResult IOperation.ReadResponse(PooledSocket socket)
        {
            return this.ReadResponse(socket);
        }

        async Task<IOperationResult> IOperation.ReadResponseAsync(PooledSocket socket)
        {
            return await this.ReadResponseAsync(socket);
        }

        async Task<bool> IOperation.ReadResponseAsync(PooledSocket socket, Action<bool> next)
        {
            return await ReadResponseAsync(socket, next);
        }

        int IOperation.StatusCode
        {
            get { return this.StatusCode; }
        }

        public int StatusCode { get; protected set; }
    }
}