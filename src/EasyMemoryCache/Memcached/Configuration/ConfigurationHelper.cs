using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EasyMemoryCache.Memcached.Configuration
{
    public static class ConfigurationHelper
    {
        internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out int value, bool required)
        {
            string tmp;
            if (TryGetAndRemove(dict, name, out tmp, required)
                && Int32.TryParse(tmp, out value))
            {
                return true;
            }

            if (required)
            {
                throw new Exception("Missing or invalid parameter: " +
                                    (String.IsNullOrEmpty(name) ? "element content" : name));
            }

            value = 0;

            return false;
        }

        internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out TimeSpan value,
            bool required)
        {
            string tmp;
            if (TryGetAndRemove(dict, name, out tmp, required)
                && TimeSpan.TryParse(tmp, out value))
            {
                return true;
            }

            if (required)
            {
                throw new Exception("Missing or invalid parameter: " +
                                    (String.IsNullOrEmpty(name) ? "element content" : name));
            }

            value = TimeSpan.Zero;

            return false;
        }

        internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out string value,
            bool required)
        {
            if (dict.TryGetValue(name, out value))
            {
                dict.Remove(name);

                if (!String.IsNullOrEmpty(value))
                {
                    return true;
                }
            }

            if (required)
            {
                throw new Exception("Missing parameter: " + (String.IsNullOrEmpty(name) ? "element content" : name));
            }

            return false;
        }

        internal static void CheckForUnknownAttributes(Dictionary<string, string> dict)
        {
            if (dict.Count > 0)
            {
                throw new Exception("Unrecognized parameter: " + dict.Keys.First());
            }
        }

        public static void CheckForInterface(Type type, Type interfaceType)
        {
            if (type == null || interfaceType == null)
            {
                return;
            }
        }

        public static DnsEndPoint ResolveToEndPoint(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            var parts = value.Split(':');
            if (parts.Length != 2)
            {
                throw new ArgumentException("host:port is expected", "value");
            }

            int port;
            if (!Int32.TryParse(parts[1], out port))
            {
                throw new ArgumentException("Cannot parse port: " + parts[1], "value");
            }

            return new DnsEndPoint(parts[0], port);
        }
    }
}