using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached;
using EasyMemoryCache.Memcached.Memcached.Results;

namespace EasyMemoryCache.Memcached
{
    /// <summary>
    /// Interface for API methods that return detailed operation results
    /// </summary>
    public interface IMemcachedResultsClient
    {
        IGetOperationResult ExecuteGet(string key);

        IGetOperationResult<T> ExecuteGet<T>(string key);

        IDictionary<string, IGetOperationResult> ExecuteGet(IEnumerable<string> keys);

        IGetOperationResult ExecuteTryGet(string key, out object value);

        IGetOperationResult ExecuteTryGet<T>(string key, out T value);

        IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value);

        IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value, DateTime expiresAt);

        IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value, TimeSpan validFor);

        IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value);

        IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, ulong cas);

        IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas);

        IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, ulong cas);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

        IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, ulong cas);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

        IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

        IConcatOperationResult ExecuteAppend(string key, ArraySegment<byte> data);

        IConcatOperationResult ExecuteAppend(string key, ulong cas, ArraySegment<byte> data);

        IConcatOperationResult ExecutePrepend(string key, ArraySegment<byte> data);

        IConcatOperationResult ExecutePrepend(string key, ulong cas, ArraySegment<byte> data);

        IRemoveOperationResult ExecuteRemove(string key);

        Task<IRemoveOperationResult> ExecuteRemoveAsync(string key);
    }
}