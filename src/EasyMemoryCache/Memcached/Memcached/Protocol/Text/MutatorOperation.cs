using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class MutatorOperation : SingleItemOperation, IMutatorOperation
    {
        private readonly MutationMode mode;
        private readonly ulong delta;
        private ulong result;

        internal MutatorOperation(MutationMode mode, string key, ulong delta)
            : base(key)
        {
            this.delta = delta;
            this.mode = mode;
        }

        public ulong Result
        {
            get { return this.result; }
        }

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            var command = (this.mode == MutationMode.Increment ? "incr " : "decr ")
                            + this.Key
                            + " "
                            + this.delta.ToString(CultureInfo.InvariantCulture)
                            + TextSocketHelper.CommandTerminator;

            return TextSocketHelper.GetCommandBuffer(command);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            string response = TextSocketHelper.ReadResponse(socket);
            var result = new TextOperationResult();

            //maybe we should throw an exception when the item is not found?
            if (String.Compare(response, "NOT_FOUND", StringComparison.Ordinal) == 0)
                return result.Fail("Failed to read response. Item not found");

            result.Success =
                UInt64.TryParse(response, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out this.result);
            return result;
        }

        MutationMode IMutatorOperation.Mode
        {
            get { return this.mode; }
        }

        ulong IMutatorOperation.Result
        {
            get { return this.result; }
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            string response = TextSocketHelper.ReadResponse(socket);
            var result = new TextOperationResult();

            //maybe we should throw an exception when the item is not found?
            if (String.Compare(response, "NOT_FOUND", StringComparison.Ordinal) == 0)
                return new ValueTask<IOperationResult>(result.Fail("Failed to read response.  Item not found"));

            result.Success =
                UInt64.TryParse(response, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out this.result);
            return new ValueTask<IOperationResult>(result);
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}