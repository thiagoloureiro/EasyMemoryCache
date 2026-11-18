namespace EasyMemoryCache
{
    public readonly record struct CacheResult<T>(bool Found, T Value)
    {
        public static CacheResult<T> Hit(T value) => new(true, value);

        public static CacheResult<T> Miss => new(false, default);
    }
}
