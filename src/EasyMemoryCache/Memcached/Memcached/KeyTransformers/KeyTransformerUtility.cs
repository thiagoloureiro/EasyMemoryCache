namespace EasyMemoryCache.Memcached.Memcached.KeyTransformers
{
    public static class KeyTransformerUtility
    {
        public static string ToSHA1Hash(string key)
        {
            return new SHA1KeyTransformer().Transform(key);
        }
    }
}