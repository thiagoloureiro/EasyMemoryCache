using System;
using System.Text;

namespace EasyMemoryCache.Memcached.Memcached.KeyTransformers
{
    /// <summary>
    /// A key transformer which converts the item keys into Base64.
    /// </summary>
    public class Base64KeyTransformer : KeyTransformerBase
    {
        public override string Transform(string key)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(key));//, Base64FormattingOptions.None);
        }
    }
}