using System;
using System.Text;

namespace EasyMemoryCache.Memcached.Memcached.KeyTransformers
{
    /// <summary>
    /// A key transformer which converts the item keys into their Tiger hash.
    /// </summary>
    public class TigerHashKeyTransformer : KeyTransformerBase
    {
        public override string Transform(string key)
        {
            TigerHash th = new TigerHash();
            byte[] data = th.ComputeHash(Encoding.Unicode.GetBytes(key));

            return Convert.ToBase64String(data);
        }
    }
}