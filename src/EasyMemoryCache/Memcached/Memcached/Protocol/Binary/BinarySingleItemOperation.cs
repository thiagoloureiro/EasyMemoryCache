using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public abstract class BinarySingleItemOperation : SingleItemOperation
    {
        protected BinarySingleItemOperation(string key) : base(key)
        {
        }

        protected abstract BinaryRequest Build();

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            return this.Build().CreateBuffer();
        }

        protected abstract IOperationResult ProcessResponse(BinaryResponse response);

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var retval = response.Read(socket);

            this.Cas = response.CAS;
            this.StatusCode = response.StatusCode;

            var result = new BinaryOperationResult()
            {
                Success = retval,
                Cas = this.Cas,
                StatusCode = this.StatusCode
            };

            IOperationResult responseResult;
            if (!(responseResult = this.ProcessResponse(response)).Success)
            {
                result.InnerResult = responseResult;
                responseResult.Combine(result);
            }

            return result;
        }

        protected internal override async ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var retval = await response.ReadAsync(socket);

            this.Cas = response.CAS;
            this.StatusCode = response.StatusCode;

            var result = new BinaryOperationResult()
            {
                Success = retval,
                Cas = this.Cas,
                StatusCode = this.StatusCode
            };

            IOperationResult responseResult;
            if (!(responseResult = this.ProcessResponse(response)).Success)
            {
                result.InnerResult = responseResult;
                responseResult.Combine(result);
            }

            return result;
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, Action<bool> next)
        {
            throw new NotImplementedException();
        }
    }
}