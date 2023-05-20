using EasyMemoryCache.Configuration;
using System;

namespace EasyMemoryCache.Extensions
{
    public static class ConvertInterval
    {
        public static int Convert(CacheTimeInterval interval, int value)
        {
            return interval switch
            {
                CacheTimeInterval.Seconds => value,
                CacheTimeInterval.Minutes => value * 60,
                CacheTimeInterval.Hours => value * 60 * 60,
                CacheTimeInterval.Days => value * 24 * 60 * 60,
                _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
            };
        }
    }
}