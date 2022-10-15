using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public abstract class BinaryOperation : Operation
    {
        protected abstract BinaryRequest Build();

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            return this.Build().CreateBuffer();
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}