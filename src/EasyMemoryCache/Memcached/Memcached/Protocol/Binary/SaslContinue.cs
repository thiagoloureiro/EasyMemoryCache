using System;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// SASL auth step.
    /// </summary>
    public class SaslContinue : SaslStep
    {
        private readonly byte[] continuation;

        public SaslContinue(ISaslAuthenticationProvider provider, byte[] continuation)
            : base(provider)
        {
            this.continuation = continuation;
        }

        protected override BinaryRequest Build()
        {
            var request = new BinaryRequest(OpCode.SaslStep)
            {
                Key = this.Provider.Type,
                Data = new ArraySegment<byte>(this.Provider.Continue(this.continuation))
            };

            return request;
        }
    }
}