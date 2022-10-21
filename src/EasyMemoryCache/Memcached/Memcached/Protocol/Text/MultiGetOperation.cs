using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Logging;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class MultiGetOperation : MultiItemOperation, IMultiGetOperation
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MultiGetOperation));

        private Dictionary<string, CacheItem> result;

        public MultiGetOperation(IList<string> keys) : base(keys)
        {
        }

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            // gets key1 key2 key3 ... keyN\r\n

            var command = "gets " + String.Join(" ", Keys.ToArray()) + TextSocketHelper.CommandTerminator;

            return TextSocketHelper.GetCommandBuffer(command);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var retval = new Dictionary<string, CacheItem>();
            var cas = new Dictionary<string, ulong>();

            try
            {
                GetResponse r;

                while ((r = GetHelper.ReadItem(socket)) != null)
                {
                    var key = r.Key;

                    retval[key] = r.Item;
                    cas[key] = r.CasValue;
                }
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            this.result = retval;
            this.Cas = cas;

            return new TextOperationResult().Pass();
        }

        Dictionary<string, CacheItem> IMultiGetOperation.Result
        {
            get { return this.result; }
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var retval = new Dictionary<string, CacheItem>();
            var cas = new Dictionary<string, ulong>();

            try
            {
                GetResponse r;

                while ((r = GetHelper.ReadItem(socket)) != null)
                {
                    var key = r.Key;

                    retval[key] = r.Item;
                    cas[key] = r.CasValue;
                }
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            this.result = retval;
            this.Cas = cas;

            return new ValueTask<IOperationResult>(new TextOperationResult().Pass());
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}