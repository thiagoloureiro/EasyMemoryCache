using System;
using System.Collections.Generic;
using EasyMemoryCache.Memcached.Memcached.Transcoders;

namespace EasyMemoryCache.Memcached.Memcached
{
    public interface IOperationFactory
    {
        IGetOperation Get(string key);

        IMultiGetOperation MultiGet(IList<string> keys);

        IStoreOperation Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas);

        IDeleteOperation Delete(string key, ulong cas);

        IMutatorOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires, ulong cas);

        IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data);

        IStatsOperation Stats(string type);

        IFlushOperation Flush();
    }
}