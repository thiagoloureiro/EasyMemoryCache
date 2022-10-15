using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class GetOperation : SingleItemOperation, IGetOperation
    {
        private CacheItem result;

        internal GetOperation(string key) : base(key)
        {
        }

        protected internal override System.Collections.Generic.IList<System.ArraySegment<byte>> GetBuffer()
        {
            var command = "gets " + this.Key + TextSocketHelper.CommandTerminator;

            return TextSocketHelper.GetCommandBuffer(command);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            GetResponse r = GetHelper.ReadItem(socket);
            var result = new TextOperationResult();

            if (r == null) return result.Fail("Failed to read response");

            this.result = r.Item;
            this.Cas = r.CasValue;

            GetHelper.FinishCurrent(socket);

            return result.Pass();
        }

        CacheItem IGetOperation.Result
        {
            get { return this.result; }
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            GetResponse r = GetHelper.ReadItem(socket);
            var result = new TextOperationResult();

            if (r == null) return new ValueTask<IOperationResult>(result.Fail("Failed to read response"));

            this.result = r.Item;
            this.Cas = r.CasValue;

            GetHelper.FinishCurrent(socket);

            return new ValueTask<IOperationResult>(result.Pass());
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}