using System;

namespace EasyMemoryCache.Memcached.Memcached.KeyTransformers
{
    public class DefaultKeyTransformer : KeyTransformerBase
    {
        private static readonly char[] ForbiddenChars =
        {
            '\u0000', '\u0001', '\u0002', '\u0003',
            '\u0004', '\u0005', '\u0006', '\u0007',
            '\u0008', '\u0009', '\u000a', '\u000b',
            '\u000c', '\u000d', '\u000e', '\u000f',
            '\u0010', '\u0011', '\u0012', '\u0013',
            '\u0014', '\u0015', '\u0016', '\u0017',
            '\u0018', '\u0019', '\u001a', '\u001b',
            '\u001c', '\u001d', '\u001e', '\u001f',
            '\u0020'
        };

        public override string Transform(string key)
        {
            if (key.IndexOfAny(ForbiddenChars) > -1)
            {
                throw new ArgumentException("Keys cannot contain the chars 0x00-0x20 and space.");
            }

            return key;
        }
    }
}