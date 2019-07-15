using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyMemoryCache.Sample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var CacheKeyNameForAsync = "unitTestStringKeyAsync";
            var CacheKeyName = "unitTestStringKey";

            var caching = SetupDI;

            Console.WriteLine("--------------");
            Console.WriteLine("Caching Lists");
            Console.WriteLine("--------------");

            var lstStringFromAsync = await await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);
            var lstString = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);

            Console.WriteLine(string.Join(",", lstStringFromAsync));
            Console.WriteLine(string.Join(",", lstString));

            var lstStringCachedFromAsync = await await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);
            var lstStringCached = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);

            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("From Cache, you can notice now GenerateList method isn't called");
            Console.WriteLine("-----------------------------------------------------------------");

            Console.WriteLine(string.Join(",", lstStringCachedFromAsync));
            Console.WriteLine(string.Join(",", lstStringCached));
        }

        private static ICaching SetupDI
        {
            get
            {
                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<ICaching, Caching>()
                    .BuildServiceProvider();

                var caching = serviceProvider.GetService<ICaching>();
                return caching;
            }
        }

        private static List<string> ReturnListOfString()
        {
            return GenerateList();
        }

        private static Task<List<string>> ReturnListOfStringAsync()
        {
            return Task.Run(GenerateList);
        }

        private static List<string> GenerateList()
        {
            Console.WriteLine("Generating the list...");
            return new List<string> { "foo", "bar", "easy", "caching" };
        }
    }
}