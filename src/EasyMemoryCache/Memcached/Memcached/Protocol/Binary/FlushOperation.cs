using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class FlushOperation : BinaryOperation, IFlushOperation
    {
        public FlushOperation()
        { }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest(OpCode.Flush);

            return request;
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var retval = response.Read(socket);

            this.StatusCode = StatusCode;
            var result = new BinaryOperationResult()
            {
                Success = retval,
                StatusCode = this.StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }

        protected internal override async ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var retval = await response.ReadAsync(socket);

            this.StatusCode = StatusCode;
            var result = new BinaryOperationResult()
            {
                Success = retval,
                StatusCode = this.StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }
    }
}