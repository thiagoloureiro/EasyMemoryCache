using System;
using System.Buffers.Binary;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Results.Helpers;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class MutatorOperation : BinarySingleItemOperation, IMutatorOperation
    {
        private readonly ulong defaultValue;
        private readonly ulong delta;
        private readonly uint expires;
        private readonly MutationMode mode;
        private ulong result;

        public MutatorOperation(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires)
            : base(key)
        {
            if (delta < 0) throw new ArgumentOutOfRangeException("delta", "delta must be >= 0");

            this.defaultValue = defaultValue;
            this.delta = delta;
            this.expires = expires;
            this.mode = mode;
        }

        protected unsafe void UpdateExtra(BinaryRequest request)
        {
            if (mode == MutationMode.Touch)
            {
                Span<byte> extra = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32BigEndian(extra, this.expires);
                request.Extra = new ArraySegment<byte>(extra.ToArray());
            }
            else
            {
                byte[] extra = new byte[20];

                fixed (byte* buffer = extra)
                {
                    BinaryConverter.EncodeUInt64(this.delta, buffer, 0);

                    BinaryConverter.EncodeUInt64(this.defaultValue, buffer, 8);
                    BinaryConverter.EncodeUInt32(this.expires, buffer, 16);
                }

                request.Extra = new ArraySegment<byte>(extra);
            }
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest((OpCode)this.mode)
            {
                Key = this.Key,
                Cas = this.Cas
            };

            this.UpdateExtra(request);

            return request;
        }

        protected override IOperationResult ProcessResponse(BinaryResponse response)
        {
            var result = new BinaryOperationResult();
            var status = response.StatusCode;
            this.StatusCode = status;

            if (status == 0)
            {
                if (mode != MutationMode.Touch)
                {
                    var data = response.Data;
                    if (data.Count != 8)
                    {
                        return result.Fail("Result must be 8 bytes long, received: " + data.Count,
                            new InvalidOperationException());
                    }

                    this.result = BinaryConverter.DecodeUInt64(data.Array, data.Offset);
                }

                return result.Pass();
            }

            var message = ResultHelper.ProcessResponseData(response.Data);
            return result.Fail(message);
        }

        MutationMode IMutatorOperation.Mode
        {
            get { return this.mode; }
        }

        ulong IMutatorOperation.Result
        {
            get { return this.result; }
        }
    }
}