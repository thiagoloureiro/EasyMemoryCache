using System;
using System.Security.Cryptography;
using System.Text;

namespace EasyMemoryCache.Memcached.Memcached.KeyTransformers
{
    /// <summary>
    /// A key transformer which converts the item keys into their SHA1 hash.
    /// </summary>
    public class SHA1KeyTransformer : KeyTransformerBase
    {
        public override string Transform(string key)
        {
            var sh = SHA1.Create();
            byte[] data = sh.ComputeHash(Encoding.Unicode.GetBytes(key));

            return Convert.ToBase64String(data);
        }
    }
}