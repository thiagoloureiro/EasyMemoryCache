using EasyMemoryCache.Memcached.Memcached.Protocol.Binary;

namespace EasyMemoryCache.Memcached.Memcached
{
    public enum MutationMode : byte
    { Increment = 0x05, Decrement = 0x06, Touch = OpCode.Touch };

    public enum ConcatenationMode : byte
    { Append = 0x0E, Prepend = 0x0F };

    public enum MemcachedProtocol
    { Binary, Text }
}