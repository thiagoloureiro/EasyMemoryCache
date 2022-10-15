using System;
using EasyMemoryCache.Memcached.Memcached.Results;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// Implements append/prepend.
    /// </summary>
    public class ConcatOperation : BinarySingleItemOperation, IConcatOperation
    {
        private readonly ArraySegment<byte> data;
        private readonly ConcatenationMode mode;

        public ConcatOperation(ConcatenationMode mode, string key, ArraySegment<byte> data)
            : base(key)
        {
            this.data = data;
            this.mode = mode;
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest((OpCode)this.mode)
            {
                Key = this.Key,
                Cas = this.Cas,
                Data = this.data
            };

            return request;
        }

        protected override IOperationResult ProcessResponse(BinaryResponse response)
        {
            return new BinaryOperationResult() { Success = true };
        }

        ConcatenationMode IConcatOperation.Mode
        {
            get { return this.mode; }
        }
    }
}