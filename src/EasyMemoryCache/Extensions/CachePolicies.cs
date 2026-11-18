using System;
using System.Collections;

namespace EasyMemoryCache.Extensions
{
    internal static class CachePolicies
    {
        public static void RequireKey(string key, string paramName)
        {
            if (key is null)
                throw new ArgumentNullException(paramName);

            if (key.Length == 0)
                throw new ArgumentException("A cache key is required.", paramName);
        }

        public static bool ShouldStore<T>(T value, bool cacheEmptyCollection)
        {
            if (value is null)
                return false;

            if (value is ICollection collection)
                return cacheEmptyCollection || collection.Count > 0;

            return true;
        }
    }
}
