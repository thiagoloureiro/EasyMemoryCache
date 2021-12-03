using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyMemoryCache
{
    public class DataContainer
    {
        public string Key { get; set; }
        public long? Size { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public CacheItemPriority Priority { get; set; }

        public DataContainer(string Key, long? Size, DateTimeOffset? AbsoluteExpiration, 
            CacheItemPriority Priority)
        {
            this.Key = Key;
            this.Size = Size;
            this.AbsoluteExpiration = AbsoluteExpiration;
            this.Priority = Priority;
        }

        public override string ToString()
        {
            return "(" + String.Join(",", Key, Size, AbsoluteExpiration, Priority) + ")";
        }
    }
}
