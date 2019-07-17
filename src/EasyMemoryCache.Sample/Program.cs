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
            var CacheKeyNameWithParamForAsync = "unitTestStringKeyWithParamAsync";

            var CacheKeyName = "unitTestStringKey";

            var caching = SetupDI;

            Console.WriteLine("--------------");
            Console.WriteLine("Caching Lists");
            Console.WriteLine("--------------");

            var lstStringFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);
            var lstString = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);
            var lstStringWithParamFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameWithParamForAsync, 20, () => ReturnListOfStringAsync("EasyMemoryCache"));

            Console.WriteLine(string.Join(",", lstStringFromAsync));
            Console.WriteLine(string.Join(",", lstString));
            Console.WriteLine(string.Join(",", lstStringWithParamFromAsync));


            var lstStringCachedFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);
            var lstStringCached = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);
            var lstStringCachedWithParamFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameWithParamForAsync, 20, () => ReturnListOfStringAsync("EasyMemoryCache"));

            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("From Cache, you can notice now GenerateList method isn't called");
            Console.WriteLine("-----------------------------------------------------------------");

            Console.WriteLine(string.Join(",", lstStringCachedFromAsync));
            Console.WriteLine(string.Join(",", lstStringCached));
            Console.WriteLine(string.Join(",", lstStringCachedWithParamFromAsync));

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

        private static Task<List<string>> ReturnListOfStringAsync(string param)
        {
            return Task.Run(() => GenerateListParam(param));
        }

        private static List<string> GenerateList()
        {
            Console.WriteLine("Generating the list...");
            return new List<string> { "foo", "bar", "easy", "caching" };
        }

        private static List<string> GenerateListParam(string param)
        {
            Console.WriteLine("Generating the list...");
            return new List<string> { "foo", "bar", "easy", "caching", param };
        }
    }
}