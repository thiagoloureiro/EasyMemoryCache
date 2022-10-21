namespace EasyMemoryCache.Memcached
{
    public interface IFastObjectFacory
    {
        object CreateInstance();
    }

    public interface IFastMultiArgObjectFacory
    {
        object CreateInstance(object[] args);
    }
}