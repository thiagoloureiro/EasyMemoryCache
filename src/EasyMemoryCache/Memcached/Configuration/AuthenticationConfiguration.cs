using System;
using System.Collections.Generic;
using EasyMemoryCache.Memcached.Memcached;

namespace EasyMemoryCache.Memcached.Configuration
{
    public class AuthenticationConfiguration : IAuthenticationConfiguration
    {
        private Type authenticator;
        private Dictionary<string, object> parameters;

        Type IAuthenticationConfiguration.Type
        {
            get { return this.authenticator; }
            set
            {
                ConfigurationHelper.CheckForInterface(value, typeof(ISaslAuthenticationProvider));
                this.authenticator = value;
            }
        }

        Dictionary<string, object> IAuthenticationConfiguration.Parameters
        {
            get { return this.parameters ?? (this.parameters = new Dictionary<string, object>()); }
        }
    }
}