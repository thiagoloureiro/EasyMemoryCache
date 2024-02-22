using System;
using System.Security.Cryptography;

namespace EasyMemoryCache.Memcached
{
    /// <summary>
    ///	This is Jenkin's "One at A time Hash".
    ///	http://en.wikipedia.org/wiki/Jenkins_hash_function
    ///
    ///	Coming from libhashkit.
    /// </summary>
    /// <remarks>Does not support block based hashing.</remarks>
    internal class HashkitOneAtATime : HashAlgorithm, IUIntHashAlgorithm
    {
        public HashkitOneAtATime()
        {
        }

        public override void Initialize()
        {
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (ibStart < 0 || ibStart > array.Length)
            {
                throw new ArgumentOutOfRangeException("ibStart");
            }

            if (ibStart + cbSize > array.Length)
            {
                throw new ArgumentOutOfRangeException("cbSize");
            }

            HashkitOneAtATime.UnsafeHashCore(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(this.CurrentHash);
        }

        public uint CurrentHash { get; private set; }

        #region [ UnsafeHashCore               ]

        // see the murmur hash about stream support
        private static unsafe uint UnsafeHashCore(byte[] data, int offset, int count)
        {
            uint hash = 0;

            fixed (byte* start = &(data[offset]))
            {
                var ptr = start;

                while (count > 0)
                {
                    hash += *ptr;
                    hash += (hash << 10);
                    hash ^= (hash >> 6);

                    count--;
                    ptr++;
                }
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return hash;
        }

        #endregion [ UnsafeHashCore               ]

        #region [ IUIntHash                    ]

        uint IUIntHashAlgorithm.ComputeHash(byte[] data)
        {
            this.Initialize();

            this.HashCore(data, 0, data.Length);

            return this.CurrentHash;
        }

        #endregion [ IUIntHash                    ]
    }
}