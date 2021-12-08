using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EasyMemoryCache.Tests
{
    public class CachingTests
    {
        private string CacheKeyName = "unitTestStringKey";
        private string CacheKeyName2 = "unitTestStringKey2";
        private string CacheKeyTestDoubleKey = "CacheKeyTestDoubleKey";
        private string CacheKeyTestIntegerKey = "CacheKeyTestIntegerKey";
        private string CacheKeyTestDateTimeKey = "CacheKeyTestDateTimeKey";

        [Fact]
        public async Task should_return_datetime_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestDateTimeKey, 20, GetDateTime);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestDateTimeKey);
            var value = (DateTime)objectTask;
            Assert.Equal(value, GetDateTime().Result);
            Assert.Equal(ret, GetDateTime().Result);
        }

        [Fact]
        public async Task should_return_int_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestIntegerKey, 20, GetInteger);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestIntegerKey);
            var value = (int)objectTask;
            Assert.Equal(value, GetInteger().Result);
            Assert.Equal(ret, GetInteger().Result);
        }

        [Fact]
        public async Task should_return_double_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestDoubleKey, 20, GetDouble);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestDoubleKey);
            var value = (double)objectTask;
            Assert.Equal(value, GetDouble().Result);
            Assert.Equal(ret, GetDouble().Result);
        }

        [Fact]
        public async Task should_return_a_list_of_string_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyName, 20, ReturnListOfStringAsync);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objectTask;
            Assert.Equal(lst, GenerateList());
            Assert.Equal(ret, GenerateList());
        }

        [Fact]
        public async Task should_notcache_empty_return_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyName, 20, ReturnEmptyListAsync);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objectTask;
            Assert.Equal(lst, null);
            Assert.Equal(ret, GenerateEmptyList());
        }

        [Fact]
        public void should_notcache_empty_return_without_parameters_sync()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnEmptyList);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objectTask;
            Assert.Equal(lst, null);
            Assert.Equal(ret, GenerateEmptyList());
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

        [Fact]
        public async Task should_return_list_of_keys()
        {
            // Arrange
            var caching = new Caching();
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyName, 20, ReturnListOfStringAsync);
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objectTask;

            // Act
            var keys = caching.GetKeys().ToList();

            // Assert
            Assert.True(keys.Count > 0);
        }

        [Fact]
        public async Task should_return_list_of_keys_complete()
        {
            // Arrange
            var caching = new Caching();
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyName, 20, ReturnListOfStringAsync);
            var objectTask = caching.GetValueFromCache(CacheKeyName);
            var lst = (List<string>)objectTask;

            // Act
            var keys = caching.GetData().ToList();

            // Assert
            Assert.True(keys.Count > 0);
        }

        private Task<List<string>> ReturnEmptyListAsync()
        {
            return Task.Run(GenerateEmptyList);
        }

        private List<string> ReturnEmptyList()
        {
            return GenerateEmptyList();
        }

        private List<string> ReturnListOfString()
        {
            return GenerateList();
        }

        private Task<List<string>> ReturnListOfStringAsync()
        {
            return Task.Run(GenerateList);
        }

        private List<string> GenerateEmptyList()
        {
            return new List<string>();
        }

        private List<string> GenerateList()
        {
            return new List<string> { "foo", "bar", "easy", "caching" };
        }

        private Task<int> GetInteger()
        {
            return Task.FromResult(200);
        }

        private Task<double> GetDouble()
        {
            return Task.FromResult(200.55);
        }

        private Task<DateTime> GetDateTime()
        {
            return Task.FromResult(new DateTime(2019, 01, 01));
        }
    }
}