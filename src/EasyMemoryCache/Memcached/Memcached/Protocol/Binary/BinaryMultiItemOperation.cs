using System;
using System.Collections.Generic;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public abstract class BinaryMultiItemOperation : MultiItemOperation
    {
        public BinaryMultiItemOperation(IList<string> keys) : base(keys)
        {
        }

        protected abstract BinaryRequest Build(string key);

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            var keys = this.Keys;
            var retval = new List<ArraySegment<byte>>(keys.Count * 2);

            foreach (var k in keys)
                this.Build(k).CreateBuffer(retval);

            return retval;
        }
    }
}