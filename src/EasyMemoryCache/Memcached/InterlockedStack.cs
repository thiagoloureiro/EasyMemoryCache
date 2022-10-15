using System;
using System.Threading;

namespace EasyMemoryCache.Memcached
{
    /// <summary>
    /// Implements a non-locking stack.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    [Obsolete]
    public class InterlockedStack<TItem>
    {
        private Node head;

        public InterlockedStack()
        {
            this.head = new Node(default(TItem));
        }

        public void Push(TItem item)
        {
            var node = new Node(item);

            do { node.Next = this.head.Next; }
            while (Interlocked.CompareExchange(ref this.head.Next, node, node.Next) != node.Next);
        }

        public bool TryPop(out TItem value)
        {
            value = default(TItem);
            Node node;

            do
            {
                node = head.Next;
                if (node == null) return false;
            }
            while (Interlocked.CompareExchange(ref head.Next, node.Next, node) != node);

            value = node.Value;

            return true;
        }

        #region [ Node                        ]

        private class Node
        {
            public readonly TItem Value;
            public Node Next;

            public Node(TItem value)
            {
                this.Value = value;
            }
        }

        #endregion [ Node                        ]
    }
}