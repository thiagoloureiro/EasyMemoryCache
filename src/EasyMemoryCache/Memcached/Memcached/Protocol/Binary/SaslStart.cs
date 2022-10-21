using System;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// Starts the SASL auth sequence.
    /// </summary>
    public class SaslStart : SaslStep
    {
        public SaslStart(ISaslAuthenticationProvider provider) : base(provider)
        {
        }

        protected override BinaryRequest Build()
        {
            // create a Sasl Start command
            var request = new BinaryRequest(OpCode.SaslStart)
            {
                Key = this.Provider.Type,
                Data = new ArraySegment<byte>(this.Provider.Authenticate())
            };

            return request;
        }
    }
}