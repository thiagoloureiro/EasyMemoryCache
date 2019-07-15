using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EasyMemoryCache.Tests
{
    public class CachingTests
    {
        private string CacheKeyName = "unitTestStringKey";
        private string CacheKeyName2 = "unitTestStringKey2";

        [Fact]
        public async Task should_return_a_list_of_string_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await await caching.GetOrSetObjectFromCacheAsync(CacheKeyName, 20, ReturnListOfStringAsync);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = await (Task<List<string>>)objectTask;
            Assert.Equal(lst, GenerateList());
            Assert.Equal(ret, GenerateList());
        }

        [Fact]
        public void should_return_a_list_of_string_without_parameters_sync()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objFromCache = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objFromCache;
            Assert.Equal(lst, GenerateList());
            Assert.Equal(ret, GenerateList());
        }

        [Fact]
        public void should_invalidate_all_cache()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);
            caching.InvalidateAll();

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objFromCache = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objFromCache;
            Assert.Null(lst);
        }

        [Fact]
        public void should_invalidate_one_key_from_cache()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);
            var ret2 = caching.GetOrSetObjectFromCache(CacheKeyName2, 20, ReturnListOfString);

            caching.Invalidate(CacheKeyName);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objFromCache = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objFromCache;

            var objFromCache2 = caching.GetValueFromCache(CacheKeyName2);
            var lst2 = (List<string>)objFromCache2;

            Assert.Null(lst);
            Assert.Equal(lst2, GenerateList());
        }

        private List<string> ReturnListOfString()
        {
            return GenerateList();
        }

        private Task<List<string>> ReturnListOfStringAsync()
        {
            return Task.Run(GenerateList);
        }

        private List<string> GenerateList()
        {
            return new List<string> { "foo", "bar", "easy", "caching" };
        }
    }
}