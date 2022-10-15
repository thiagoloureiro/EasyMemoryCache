using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Results.Helpers;
using EasyMemoryCache.Memcached.Memcached.Transcoders;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class GetOperation : BinarySingleItemOperation, IGetOperation
    {
        private readonly ILogger _logger;
        private CacheItem result;

        public GetOperation(string key, ILogger logger) : base(key)
        {
            _logger = logger;
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest(OpCode.Get)
            {
                Key = this.Key,
                Cas = this.Cas
            };

            return request;
        }

        protected override IOperationResult ProcessResponse(BinaryResponse response)
        {
            var status = response.StatusCode;
            var result = new BinaryOperationResult();

            this.StatusCode = status;

            if (status == 0)
            {
                int flags = BinaryConverter.DecodeInt32(response.Extra, 0);
                this.result = new CacheItem((ushort)flags, response.Data);
                this.Cas = response.CAS;

#if EVEN_MORE_LOGGING
                if(_logger.IsEnabled(LogLevel.Debug))
			        _logger.LogDebug("Get succeeded for key '{0}'.", this.Key);
#endif

                return result.Pass();
            }

            this.Cas = 0;

#if EVEN_MORE_LOGGING
            if(_logger.IsEnabled(LogLevel.Debug))
			    _logger.LogDebug("Get failed for key '{0}'. Reason: {1}", this.Key, Encoding.ASCII.GetString(response.Data.Array, response.Data.Offset, response.Data.Count));
#endif

            var message = ResultHelper.ProcessResponseData(response.Data);
            return result.Fail(message);
        }

        CacheItem IGetOperation.Result
        {
            get { return this.result; }
        }
    }
}