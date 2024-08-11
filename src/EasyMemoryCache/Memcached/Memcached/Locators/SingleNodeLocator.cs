using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyMemoryCache.Memcached.Memcached.Locators
{
    /// <summary>
    /// This is a simple node locator with no computation overhead, always returns the first server from the list. Use only in single server deployments.
    /// </summary>
    public sealed class SingleNodeLocator : IMemcachedNodeLocator
    {
        private IMemcachedNode node;
        private bool isInitialized;

        void IMemcachedNodeLocator.Initialize(IList<IMemcachedNode> nodes)
        {
            if (nodes.Count > 0)
            {
                node = nodes[0];
            }

            this.isInitialized = true;
        }

        IMemcachedNode IMemcachedNodeLocator.Locate(string key)
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("You must call Initialize first");
            }

            if (this.node == null)
            {
                return null;
            }

            return this.node;
        }

        IEnumerable<IMemcachedNode> IMemcachedNodeLocator.GetWorkingNodes()
        {
            return this.node.IsAlive
                ? new IMemcachedNode[] { this.node }
                : Enumerable.Empty<IMemcachedNode>();
        }
    }
}