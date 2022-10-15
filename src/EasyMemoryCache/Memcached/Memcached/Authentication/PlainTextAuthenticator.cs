using System.Collections.Generic;

namespace EasyMemoryCache.Memcached.Memcached.Authentication
{
    /// <summary>
    /// Implements the default plain text ("PLAIN") Memcached authentication.
    /// </summary>
    /// <remarks>Either use the parametrized constructor, or pass the "userName" and "password" parameters during initalization.</remarks>
    public sealed class PlainTextAuthenticator : ISaslAuthenticationProvider
    {
        private byte[] authData;

        public PlainTextAuthenticator()
        { }

        public PlainTextAuthenticator(string zone, string userName, string password)
        {
            this.authData = CreateAuthData(zone, userName, password);
        }

        string ISaslAuthenticationProvider.Type
        {
            get { return "PLAIN"; }
        }

        void ISaslAuthenticationProvider.Initialize(Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                this.authData = CreateAuthData(
                    GetParameter(parameters, "zone"),
                    GetParameter(parameters, "userName"),
                    GetParameter(parameters, "password"));
            }
        }

        private string GetParameter(Dictionary<string, object> parameters, string key)
        {
            if (parameters.ContainsKey(key))
            {
                return (string)parameters[key];
            }
            else
            {
                throw new MemcachedClientException($"Unable to find '{key}' authentication parameter for {nameof(PlainTextAuthenticator)}");
            }
        }

        byte[] ISaslAuthenticationProvider.Authenticate()
        {
            return this.authData;
        }

        byte[] ISaslAuthenticationProvider.Continue(byte[] data)
        {
            return null;
        }

        private static byte[] CreateAuthData(string zone, string userName, string password)
        {
            //message   = [authzid] UTF8NUL authcid UTF8NUL passwd
            //authcid   = 1*SAFE ; MUST accept up to 255 octets
            //authzid   = 1*SAFE ; MUST accept up to 255 octets
            //passwd    = 1*SAFE ; MUST accept up to 255 octets
            //UTF8NUL   = %x00 ; UTF-8 encoded NUL character
            return System.Text.Encoding.UTF8.GetBytes(zone + "\0" + userName + "\0" + password);
        }
    }
}