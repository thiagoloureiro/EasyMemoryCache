using System;
using System.Collections.Generic;
using EasyMemoryCache.Memcached.Configuration;
using EasyMemoryCache.Memcached.Logging;

namespace EasyMemoryCache.Memcached.Memcached.FailurePolicy
{
    /// <summary>
    /// Fails a node when the specified number of failures happen in a specified time window.
    /// </summary>
    public class ThrottlingFailurePolicy : INodeFailurePolicy
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ThrottlingFailurePolicy));
        private static readonly bool LogIsDebugEnabled = log.IsDebugEnabled;

        private readonly int resetAfter;
        private readonly int failureThreshold;
        private DateTime lastFailed;
        private int failCounter;

        /// <summary>
        /// Creates a new instance of <see cref="T:ThrottlingFailurePolicy"/>.
        /// </summary>
        /// <param name="resetAfter">Specifies the time in milliseconds how long a node should function properly to reset its failure counter.</param>
        /// <param name="failureThreshold">Specifies the number of failures that must occur in the specified time window to fail a node.</param>
        public ThrottlingFailurePolicy(int resetAfter, int failureThreshold)
        {
            this.resetAfter = resetAfter;
            this.failureThreshold = failureThreshold;
        }

        bool INodeFailurePolicy.ShouldFail()
        {
            var now = DateTime.UtcNow;

            if (lastFailed == DateTime.MinValue)
            {
                if (LogIsDebugEnabled)
                {
                    log.Debug("Setting fail counter to 1.");
                }

                failCounter = 1;
            }
            else
            {
                var diff = (int)(now - lastFailed).TotalMilliseconds;
                if (LogIsDebugEnabled)
                {
                    log.DebugFormat("Last fail was {0} msec ago with counter {1}.", diff, this.failCounter);
                }

                if (diff <= this.resetAfter)
                {
                    this.failCounter++;
                }
                else
                {
                    this.failCounter = 1;
                }
            }

            lastFailed = now;

            if (this.failCounter == this.failureThreshold)
            {
                if (LogIsDebugEnabled)
                {
                    log.DebugFormat("Threshold reached, node will fail.");
                }

                this.lastFailed = DateTime.MinValue;
                this.failCounter = 0;

                return true;
            }

            if (LogIsDebugEnabled)
            {
                log.DebugFormat("Current counter is {0}, threshold not reached.", this.failCounter);
            }

            return false;
        }
    }

    /// <summary>
    /// Creates instances of <see cref="T:ThrottlingFailurePolicy"/>.
    /// </summary>
    public class ThrottlingFailurePolicyFactory : INodeFailurePolicyFactory, IProviderFactory<INodeFailurePolicyFactory>
    {
        public ThrottlingFailurePolicyFactory(int failureThreshold, TimeSpan resetAfter)
            : this(failureThreshold, (int)resetAfter.TotalMilliseconds)
        {
        }

        public ThrottlingFailurePolicyFactory(int failureThreshold, int resetAfter)
        {
            this.ResetAfter = resetAfter;
            this.FailureThreshold = failureThreshold;
        }

        // used by the config files
        internal ThrottlingFailurePolicyFactory()
        {
        }

        /// <summary>
        /// Gets or sets the amount of time of in milliseconds after the failure counter is reset.
        /// </summary>
        public int ResetAfter { get; private set; }

        /// <summary>
        /// Gets or sets the number of failures that must happen in a time window to make a node marked as failed.
        /// </summary>
        public int FailureThreshold { get; private set; }

        INodeFailurePolicy INodeFailurePolicyFactory.Create(IMemcachedNode node)
        {
            return new ThrottlingFailurePolicy(this.ResetAfter, this.FailureThreshold);
        }

        INodeFailurePolicyFactory IProviderFactory<INodeFailurePolicyFactory>.Create()
        {
            return new ThrottlingFailurePolicyFactory(this.FailureThreshold, this.ResetAfter);
        }

        void IProvider.Initialize(Dictionary<string, string> parameters)
        {
            int failureThreshold;
            ConfigurationHelper.TryGetAndRemove(parameters, "failureThreshold", out failureThreshold, true);

            if (failureThreshold < 1)
            {
                throw new InvalidOperationException("failureThreshold must be > 0");
            }

            this.FailureThreshold = failureThreshold;

            TimeSpan reset;
            ConfigurationHelper.TryGetAndRemove(parameters, "resetAfter", out reset, true);
            if (reset <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("resetAfter must be > 0msec");
            }

            this.ResetAfter = (int)reset.TotalMilliseconds;
        }
    }
}