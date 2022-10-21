namespace EasyMemoryCache.Memcached.Memcached
{
    public interface IAuthenticator
    {
        bool Authenticate(PooledSocket socket);
    }
}