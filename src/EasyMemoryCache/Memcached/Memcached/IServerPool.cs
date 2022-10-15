using System;
using System.Collections.Generic;

namespace EasyMemoryCache.Memcached.Memcached
{
    /// <summary>
    /// Provides custom server pool implementations
    /// </summary>
    public interface IServerPool : IDisposable
    {
        IMemcachedNode Locate(string key);

        IOperationFactory OperationFactory { get; }

        IEnumerable<IMemcachedNode> GetWorkingNodes();

        void Start();

        event Action<IMemcachedNode> NodeFailed;
    }
}