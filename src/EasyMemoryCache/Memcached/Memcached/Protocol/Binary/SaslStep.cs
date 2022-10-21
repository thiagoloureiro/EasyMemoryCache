using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public abstract class SaslStep : BinaryOperation
    {
        protected SaslStep(ISaslAuthenticationProvider provider)
        {
            this.Provider = provider;
        }

        protected ISaslAuthenticationProvider Provider { get; private set; }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var response = new BinaryResponse();

            var retval = response.Read(socket);

            this.StatusCode = response.StatusCode;
            this.Data = response.Data.Array;

            var result = new BinaryOperationResult
            {
                StatusCode = this.StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }

        protected internal override async ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var response = new BinaryResponse();

            var retval = await response.ReadAsync(socket);

            this.StatusCode = response.StatusCode;
            this.Data = response.Data.Array;

            var result = new BinaryOperationResult
            {
                StatusCode = this.StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }

        public byte[] Data { get; private set; }
    }
}