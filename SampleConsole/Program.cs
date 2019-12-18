using System;
using System.Collections.Generic;
using EasyMemoryCache;
using Microsoft.Extensions.DependencyInjection;

namespace TEstBla
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICaching, Caching>()
                .BuildServiceProvider();

            var caching = serviceProvider.GetService<ICaching>();

            var lstString = caching.GetOrSetObjectFromCache("Test", 20, GenerateList);

            Console.WriteLine(string.Join(",", lstString));
            var lstString2 = caching.GetOrSetObjectFromCache("Test", 20, GenerateList);
            Console.WriteLine(string.Join(",", lstString2));
        }

        private static List<string> GenerateList()
        {
            Console.WriteLine("test");
            return new List<string> { "foo", "bar", "easy", "caching" };
        }
    }
}