using System;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Results.Helpers;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class StoreOperation : BinarySingleItemOperation, IStoreOperation
    {
        private readonly StoreMode mode;
        private CacheItem value;
        private readonly uint expires;

        public StoreOperation(StoreMode mode, string key, CacheItem value, uint expires) :
            base(key)
        {
            this.mode = mode;
            this.value = value;
            this.expires = expires;
        }

        protected override BinaryRequest Build()
        {
            OpCode op;
            switch (this.mode)
            {
                case StoreMode.Add: op = OpCode.Add; break;
                case StoreMode.Set: op = OpCode.Set; break;
                case StoreMode.Replace: op = OpCode.Replace; break;
                default: throw new ArgumentOutOfRangeException("mode", mode + " is not supported");
            }

            var extra = new byte[8];

            BinaryConverter.EncodeUInt32((uint)this.value.Flags, extra, 0);
            BinaryConverter.EncodeUInt32(expires, extra, 4);

            var request = new BinaryRequest(op)
            {
                Key = this.Key,
                Cas = this.Cas,
                Extra = new ArraySegment<byte>(extra),
                Data = this.value.Data
            };

            return request;
        }

        protected override IOperationResult ProcessResponse(BinaryResponse response)
        {
            var result = new BinaryOperationResult();

            this.StatusCode = response.StatusCode;
            if (response.StatusCode == 0)
            {
                return result.Pass();
            }
            else
            {
                var message = ResultHelper.ProcessResponseData(response.Data);
                return result.Fail(message);
            }
        }

        StoreMode IStoreOperation.Mode
        {
            get { return this.mode; }
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}