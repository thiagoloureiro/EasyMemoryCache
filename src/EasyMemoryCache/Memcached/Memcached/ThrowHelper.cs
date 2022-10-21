using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EasyMemoryCache.Memcached.Memcached
{
    internal static class ThrowHelper
    {
        public static void ThrowSocketWriteError(EndPoint endpoint)
        {
            throw new IOException(string.Format("Failed to write to the socket '{0}'.", endpoint));
        }

        public static void ThrowSocketWriteError(EndPoint endpoint, SocketError error)
        {
            // move the string into resource file
            throw new IOException(string.Format("Failed to write to the socket '{0}'. Error: {1}", endpoint, error));
        }
    }
}