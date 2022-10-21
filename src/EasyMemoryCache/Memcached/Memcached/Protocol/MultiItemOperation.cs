using System.Collections.Generic;

namespace EasyMemoryCache.Memcached.Memcached.Protocol
{
    /// <summary>
    /// Base class for implementing operations working with multiple items.
    /// </summary>
    public abstract class MultiItemOperation : Operation, IMultiItemOperation
    {
        public MultiItemOperation(IList<string> keys)
        {
            this.Keys = keys;
        }

        //Input
        public IList<string> Keys { get; private set; }

        // Output
        public Dictionary<string, ulong> Cas { get; protected set; }

        IList<string> IMultiItemOperation.Keys { get { return this.Keys; } }
        Dictionary<string, ulong> IMultiItemOperation.Cas { get { return this.Cas; } }
    }
}