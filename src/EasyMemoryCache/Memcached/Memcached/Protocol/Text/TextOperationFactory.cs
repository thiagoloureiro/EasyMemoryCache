using System;
using System.Collections.Generic;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class TextOperationFactory : IOperationFactory
    {
        IGetOperation IOperationFactory.Get(string key)
        {
            return new GetOperation(key);
        }

        IMultiGetOperation IOperationFactory.MultiGet(IList<string> keys)
        {
            return new MultiGetOperation(keys);
        }

        IStoreOperation IOperationFactory.Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas)
        {
            if (cas == 0)
            {
            }

            return new StoreOperation(mode, key, value, expires);

            return new CasOperation(key, value, expires, (uint)cas);
        }

        IDeleteOperation IOperationFactory.Delete(string key, ulong cas)
        {
            if (cas > 0)
            {
                throw new NotSupportedException("Text protocol does not support delete with cas.");
            }

            return new DeleteOperation(key);
        }

        IMutatorOperation IOperationFactory.Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta,
            uint expires, ulong cas)
        {
            if (cas > 0)
            {
                throw new NotSupportedException("Text protocol does not support " + mode + " with cas.");
            }

            return new MutatorOperation(mode, key, delta);
        }

        IConcatOperation IOperationFactory.Concat(ConcatenationMode mode, string key, ulong cas,
            ArraySegment<byte> data)
        {
            if (cas > 0)
            {
                throw new NotSupportedException("Text protocol does not support " + mode + " with cas.");
            }

            return new ConcateOperation(mode, key, data);
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