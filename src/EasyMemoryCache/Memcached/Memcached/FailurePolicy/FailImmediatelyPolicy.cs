namespace EasyMemoryCache.Memcached.Memcached.FailurePolicy
{
    /// <summary>
    /// Fails a node immediately when an error occures. This is the default policy.
    /// </summary>
    public sealed class FailImmediatelyPolicy : INodeFailurePolicy
    {
        bool INodeFailurePolicy.ShouldFail()
        {
            return true;
        }
    }

    /// <summary>
    /// Creates instances of <see cref="T:FailImmediatelyPolicy"/>.
    /// </summary>
    public class FailImmediatelyPolicyFactory : INodeFailurePolicyFactory
    {
        private static readonly INodeFailurePolicy PolicyInstance = new FailImmediatelyPolicy();

        INodeFailurePolicy INodeFailurePolicyFactory.Create(IMemcachedNode node)
        {
            return PolicyInstance;
        }
    }
}