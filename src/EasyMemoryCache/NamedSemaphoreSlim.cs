using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EasyMemoryCache
{
    internal class NamedSemaphoreSlim : IDisposable
    {
        private bool _disposed;
        private readonly int _initialCount;
        private readonly int _maxCount;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;

        public NamedSemaphoreSlim() : this(1)
        {

        }

        public NamedSemaphoreSlim(int initialCount) : this(initialCount, initialCount)
        {
            
        }

        public NamedSemaphoreSlim(int initialCount, int maxCount)
        {
            _initialCount = initialCount;
            _maxCount = maxCount;
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public SemaphoreSlim this[string name]
        {
            get
            {
                if (!_semaphores.ContainsKey(name))
                {
                    _semaphores.TryAdd(name, new SemaphoreSlim(_initialCount, _maxCount));
                }

                return _semaphores[name];
            }
        }

        public void Dispose() => Dispose(true);

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var semaphore in _semaphores.Values)
                    {
                        semaphore.Dispose();
                    }
                }

                _disposed = true;
            }
        }
    }
}
