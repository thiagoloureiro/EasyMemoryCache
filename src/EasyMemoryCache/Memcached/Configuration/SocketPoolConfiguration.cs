using System;
using EasyMemoryCache.Memcached.Memcached.FailurePolicy;

namespace EasyMemoryCache.Memcached.Configuration
{
    public class SocketPoolConfiguration : ISocketPoolConfiguration
    {
        private int minPoolSize = 5;
        private int maxPoolSize = 100;
        private bool useSslStream = false;
        private TimeSpan connectionTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan receiveTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan deadTimeout = new TimeSpan(0, 0, 10);
        private TimeSpan queueTimeout = new TimeSpan(0, 0, 0, 0, 100);
        private TimeSpan _initPoolTimeout = new TimeSpan(0, 1, 0);
        private INodeFailurePolicyFactory FailurePolicyFactory = new ThrottlingFailurePolicyFactory(5, TimeSpan.FromMilliseconds(2000));

        int ISocketPoolConfiguration.MinPoolSize
        {
            get { return this.minPoolSize; }
            set { this.minPoolSize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the maximum amount of sockets per server in the socket pool.
        /// </summary>
        /// <returns>The maximum amount of sockets per server in the socket pool. The default is 20.</returns>
        /// <remarks>It should be 0.75 * (number of threads) for optimal performance.</remarks>
        int ISocketPoolConfiguration.MaxPoolSize
        {
            get { return this.maxPoolSize; }
            set { this.maxPoolSize = value; }
        }

        TimeSpan ISocketPoolConfiguration.ConnectionTimeout
        {
            get { return this.connectionTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this.connectionTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.ReceiveTimeout
        {
            get { return this.receiveTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this.receiveTimeout = value;
            }
        }

        TimeSpan ISocketPoolConfiguration.QueueTimeout
        {
            get { return this.queueTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this.queueTimeout = value;
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
            get { return this.deadTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException("value", "value must be positive");

                this.deadTimeout = value;
            }
        }

        INodeFailurePolicyFactory ISocketPoolConfiguration.FailurePolicyFactory
        {
            get { return this.FailurePolicyFactory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.FailurePolicyFactory = value;
            }
        }
    }
}