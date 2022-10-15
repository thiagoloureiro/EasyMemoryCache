using System;
using System.Collections.Generic;
using EasyMemoryCache.Memcached.Memcached.Transcoders;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// Memcached client.
    /// </summary>
    public class BinaryOperationFactory : IOperationFactory
    {
        private readonly ILogger _logger;

        public BinaryOperationFactory(ILogger logger)
        {
            _logger = logger;
        }

        IGetOperation IOperationFactory.Get(string key)
        {
            return new GetOperation(key, _logger);
        }

        IMultiGetOperation IOperationFactory.MultiGet(IList<string> keys)
        {
            return new MultiGetOperation(keys);
        }

        IStoreOperation IOperationFactory.Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas)
        {
            return new StoreOperation(mode, key, value, expires) { Cas = cas };
        }

        IDeleteOperation IOperationFactory.Delete(string key, ulong cas)
        {
            return new DeleteOperation(key) { Cas = cas };
        }

        IMutatorOperation IOperationFactory.Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires, ulong cas)
        {
            return new MutatorOperation(mode, key, defaultValue, delta, expires) { Cas = cas };
        }

        IConcatOperation IOperationFactory.Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
        {
            return new ConcatOperation(mode, key, data) { Cas = cas };
        }

        IStatsOperation IOperationFactory.Stats(string type)
        {
            return new StatsOperation(type);
        }

        IFlushOperation IOperationFactory.Flush()
        {
            return new FlushOperation();
        }
    }
}