using EasyMemoryCache.Memcached.Logging;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Results.Helpers;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class DeleteOperation : BinarySingleItemOperation, IDeleteOperation
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DeleteOperation));

        public DeleteOperation(string key) : base(key)
        {
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest(OpCode.Delete)
            {
                Key = this.Key,
                Cas = this.Cas
            };

            return request;
        }

        protected override IOperationResult ProcessResponse(BinaryResponse response)
        {
            var result = new BinaryOperationResult();
#if EVEN_MORE_LOGGING
			if (log.IsDebugEnabled)
				if (response.StatusCode == 0)
					log.DebugFormat("Delete succeeded for key '{0}'.", this.Key);
				else
					log.DebugFormat("Delete failed for key '{0}'. Reason: {1}", this.Key, Encoding.ASCII.GetString(response.Data.Array, response.Data.Offset, response.Data.Count));
#endif
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
    }
}