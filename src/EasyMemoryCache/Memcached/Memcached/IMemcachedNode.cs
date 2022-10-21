using System;
using System.Net;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Memcached.Results;

namespace EasyMemoryCache.Memcached.Memcached
{
    public interface IMemcachedNode : IDisposable
    {
        EndPoint EndPoint { get; }
        bool IsAlive { get; }

        bool Ping();

        IOperationResult Execute(IOperation op);

        Task<IOperationResult> ExecuteAsync(IOperation op);

        Task<bool> ExecuteAsync(IOperation op, Action<bool> next);

        //	PooledSocket CreateSocket(TimeSpan connectionTimeout, TimeSpan receiveTimeout);

        event Action<IMemcachedNode> Failed;

        //IAsyncResult BeginExecute(IOperation op, AsyncCallback callback, object state);
        //bool EndExecute(IAsyncResult result);
    }
}