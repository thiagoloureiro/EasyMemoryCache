namespace EasyMemoryCache.Memcached
{
    internal interface IUIntHashAlgorithm
    {
        uint ComputeHash(byte[] data);
    }
}