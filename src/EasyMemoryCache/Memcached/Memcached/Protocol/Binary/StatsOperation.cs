using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Logging;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    public class StatsOperation : BinaryOperation, IStatsOperation
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StatsOperation));

        private readonly string type;
        private Dictionary<string, string> result;

        public StatsOperation(string type)
        {
            this.type = type;
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest(OpCode.Stat);
            if (!String.IsNullOrEmpty(this.type))
            {
                request.Key = this.type;
            }

            return request;
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var serverData = new Dictionary<string, string>();
            var retval = false;

            while (response.Read(socket) && response.KeyLength > 0)
            {
                retval = true;

                var data = response.Data;
                var key = BinaryConverter.DecodeKey(data.Array, data.Offset, response.KeyLength);
                var value = BinaryConverter.DecodeKey(data.Array, data.Offset + response.KeyLength,
                    data.Count - response.KeyLength);

                serverData[key] = value;
            }

            this.result = serverData;
            this.StatusCode = response.StatusCode;

            var result = new BinaryOperationResult()
            {
                StatusCode = StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }

        protected internal override async ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var response = new BinaryResponse();
            var serverData = new Dictionary<string, string>();
            var retval = false;

            while ((await response.ReadAsync(socket)) && response.KeyLength > 0)
            {
                retval = true;

                var data = response.Data;
                var key = BinaryConverter.DecodeKey(data.Array, data.Offset, response.KeyLength);
                var value = BinaryConverter.DecodeKey(data.Array, data.Offset + response.KeyLength,
                    data.Count - response.KeyLength);

                serverData[key] = value;
            }

            this.result = serverData;
            this.StatusCode = response.StatusCode;

            var result = new BinaryOperationResult()
            {
                StatusCode = StatusCode
            };

            result.PassOrFail(retval, "Failed to read response");
            return result;
        }

        Dictionary<string, string> IStatsOperation.Result
        {
            get { return this.result; }
        }
    }
}