using System;
using EasyMemoryCache.Memcached.Memcached.FailurePolicy;

namespace EasyMemoryCache.Memcached.Configuration
{
    public class SocketPoolConfiguration : ISocketPoolConfiguration
    {
        private int _minPoolSize = 5;
        private int _maxPoolSize = 100;
        private TimeSpan _connectionTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan _receiveTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan _deadTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan _queueTimeout = new TimeSpan(0, 0, 0, 0, 100);
        private TimeSpan _initPoolTimeout = new TimeSpan(0, 1, 0);

        private INodeFailurePolicyFactory _failurePolicyFactory =
            new ThrottlingFailurePolicyFactory(5, TimeSpan.FromMilliseconds(2000));

        int ISocketPoolConfiguration.MinPoolSize
        {
            get { return this._minPoolSize; }
            set { this._minPoolSize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the maximum amount of sockets per server in the socket pool.
        /// </summary>
        /// <returns>The maximum amount of sockets per server in the socket pool. The default is 20.</returns>
        /// <remarks>It should be 0.75 * (number of threads) for optimal performance.</remarks>
        int ISocketPoolConfiguration.MaxPoolSize
        {
            get { return this._maxPoolSize; }
            set { this._maxPoolSize = value; }
        }

        TimeSpan ISocketPoolConfiguration.ConnectionTimeout
        {
            get { return this._connectionTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", "value must be positive");
                }

                this._connectionTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.ReceiveTimeout
        {
            get { return this._receiveTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("value", "value must be positive");
                }

                this._receiveTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.QueueTimeout
        {
            get { return this._queueTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this._queueTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.InitPoolTimeout
        {
            get { return _initPoolTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                _initPoolTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.DeadTimeout
        {
            get { return this._deadTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this._deadTimeout = value;
            }
        }

        INodeFailurePolicyFactory ISocketPoolConfiguration.FailurePolicyFactory
        {
            get { return this._failurePolicyFactory; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this._failurePolicyFactory = value;
            }
        }
    }
}