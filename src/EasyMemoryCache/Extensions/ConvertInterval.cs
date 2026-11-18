using EasyMemoryCache.Configuration;
using System;

namespace EasyMemoryCache.Extensions
{
    public static class ConvertInterval
    {
        public static int Convert(CacheTimeInterval interval, int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Cache time must be greater than zero.");

            long seconds = interval switch
            {
                CacheTimeInterval.Seconds => value,
                CacheTimeInterval.Minutes => (long)value * 60,
                CacheTimeInterval.Hours => (long)value * 60 * 60,
                CacheTimeInterval.Days => (long)value * 24 * 60 * 60,
                _ => throw new ArgumentOutOfRangeException(nameof(interval), interval, null)
            };

            if (seconds > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Cache time is too large.");

            return (int)seconds;
        }
    }
}