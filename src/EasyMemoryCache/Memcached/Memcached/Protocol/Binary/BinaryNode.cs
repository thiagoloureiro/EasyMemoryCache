using System.Net;
using System.Security;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Configuration;
using Microsoft.Extensions.Logging;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Binary
{
    /// <summary>
    /// A node which is used by the BinaryPool. It implements the binary protocol's SASL auth. mechanism.
    /// </summary>
    public class BinaryNode : MemcachedNode
    {
        private readonly ILogger _logger;
        private readonly ISaslAuthenticationProvider authenticationProvider;

        public BinaryNode(
            EndPoint endpoint,
            ISocketPoolConfiguration config,
            ISaslAuthenticationProvider authenticationProvider,
            ILogger logger,
            bool useSslStream)
            : base(endpoint, config, logger, useSslStream)
        {
            this.authenticationProvider = authenticationProvider;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates the new socket before it is put into the pool.
        /// </summary>
        protected internal override PooledSocket CreateSocket()
        {
            var retval = base.CreateSocket();

            if (this.authenticationProvider != null && !Auth(retval))
            {
                _logger.LogError("Authentication failed: " + this.EndPoint);

                throw new SecurityException("auth failed: " + this.EndPoint);
            }

            return retval;
        }

        protected internal override async Task<PooledSocket> CreateSocketAsync()
        {
            var retval = await base.CreateSocketAsync();

            if (this.authenticationProvider != null && !(await AuthAsync(retval)))
            {
                _logger.LogError("Authentication failed: " + this.EndPoint);

                throw new SecurityException("auth failed: " + this.EndPoint);
            }

            return retval;
        }

        /// <summary>
        /// Implements memcached's SASL auth sequence. (See the protocol docs for more details.)
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private bool Auth(PooledSocket socket)
        {
            SaslStep currentStep = new SaslStart(this.authenticationProvider);

            socket.Write(currentStep.GetBuffer());

            while (!currentStep.ReadResponse(socket).Success)
            {
                // challenge-response authentication
                if (currentStep.StatusCode == 0x21)
                {
                    currentStep = new SaslContinue(this.authenticationProvider, currentStep.Data);
                    socket.Write(currentStep.GetBuffer());
                }
                else
                {
                    _logger.LogWarning("Authentication failed, return code: 0x{0:x}", currentStep.StatusCode);

                    // invalid credentials or other error
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> AuthAsync(PooledSocket socket)
        {
            SaslStep currentStep = new SaslStart(this.authenticationProvider);

            await socket.WriteAsync(currentStep.GetBuffer());

            while (!(await currentStep.ReadResponseAsync(socket)).Success)
            {
                // challenge-response authentication
                if (currentStep.StatusCode == 0x21)
                {
                    currentStep = new SaslContinue(this.authenticationProvider, currentStep.Data);
                    await socket.WriteAsync(currentStep.GetBuffer());
                }
                else
                {
                    _logger.LogWarning("Authentication failed, return code: 0x{0:x}", currentStep.StatusCode);

                    // invalid credentials or other error
                    return false;
                }
            }

            return true;
        }
    }
}