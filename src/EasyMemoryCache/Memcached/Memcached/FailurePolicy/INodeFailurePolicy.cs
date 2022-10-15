namespace EasyMemoryCache.Memcached.Memcached.FailurePolicy
{
    public interface INodeFailurePolicy
    {
        bool ShouldFail();
    }

    public interface INodeFailurePolicyFactory
    {
        INodeFailurePolicy Create(IMemcachedNode node);
    }
}