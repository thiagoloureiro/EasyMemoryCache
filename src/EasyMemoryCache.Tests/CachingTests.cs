using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyMemoryCache.Configuration;
using EasyMemoryCache.Memorycache;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace EasyMemoryCache.Tests
{
    public class CachingTests
    {
        private readonly string CacheKeyName = "unitTestStringKey";
        private readonly string CacheKeyName2 = "unitTestStringKey2";
        private readonly string CacheKeyTestDoubleKey = "CacheKeyTestDoubleKey";
        private readonly string CacheKeyTestIntegerKey = "CacheKeyTestIntegerKey";
        private readonly string CacheKeyTestDateTimeKey = "CacheKeyTestDateTimeKey";

        [Fact]
        public async Task should_return_datetime_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestDateTimeKey, 20, GetDateTime, interval: CacheTimeInterval.Seconds);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestDateTimeKey);
            var value = (DateTime)objectTask;
            Assert.Equal(value, GetDateTime().Result);
            Assert.Equal(ret, GetDateTime().Result);
        }

        [Fact]
        public async Task should_return_datetime_without_parameters_minutes_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestDateTimeKey, 20, GetDateTime, interval: CacheTimeInterval.Minutes);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestDateTimeKey);
            var value = (DateTime)objectTask;
            Assert.Equal(value, GetDateTime().Result);
            Assert.Equal(ret, GetDateTime().Result);
        }

        [Fact]
        public async Task should_return_datetime_without_parameters_hours_async()
        {
            // Arrange
            var caching = new Caching();

            // Act
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestDateTimeKey, 20, GetDateTime, interval: CacheTimeInterval.Hours);

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
            var ret = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestIntegerKey, 20, GetInteger, interval: CacheTimeInterval.Minutes);

            // Assert
            // Only for asserting purposes, no need to use GetValueFromCache, just use the GetOrSetObjectFromCacheAsync
            var objectTask = caching.GetValueFromCache(CacheKeyTestIntegerKey);
            var value = (int)objectTask;
            Assert.Equal(value, GetInteger().Result);
            Assert.Equal(ret, GetInteger().Result);
        }
        
        [Fact]
        public async Task should_set_and_return_int_without_parameters_async()
        {
            // Arrange
            var caching = new Caching();
            var intValue = 10;

            // Act
            await caching.SetValueToCacheAsync(CacheKeyTestIntegerKey, intValue, 20);

            // Assert
            var result = await caching.GetValueFromCacheAsync<int>(CacheKeyTestIntegerKey);
            Assert.Equal(intValue, result);
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
            Assert.Null(lst);
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
            Assert.Null(lst);
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
        public async Task should_keep_cached_zero()
        {
            var caching = new Caching();
            var calls = 0;

            Task<int> Factory()
            {
                calls++;
                return Task.FromResult(0);
            }

            var first = await caching.GetOrSetObjectFromCacheAsync(CacheKeyTestIntegerKey, 20, Factory);
            var second = caching.GetOrSetObjectFromCache(CacheKeyTestIntegerKey, 20, () =>
            {
                calls++;
                return 0;
            });

            Assert.Equal(0, first);
            Assert.Equal(0, second);
            Assert.Equal(1, calls);
            Assert.True(caching.TryGetValue<int>(CacheKeyTestIntegerKey, out var stored));
            Assert.Equal(0, stored);
        }

        [Fact]
        public async Task should_call_factory_once_when_requests_overlap()
        {
            var caching = new Caching();
            var calls = 0;

            async Task<string> Factory(CancellationToken cancellationToken)
            {
                Interlocked.Increment(ref calls);
                await Task.Delay(50, cancellationToken);
                return "value";
            }

            var results = await Task.WhenAll(Enumerable.Range(0, 20)
                .Select(_ => caching.GetOrSetObjectFromCacheAsync("stampede", 30, Factory)));

            Assert.All(results, value => Assert.Equal("value", value));
            Assert.Equal(1, calls);
        }

        [Fact]
        public async Task should_surface_factory_exception()
        {
            var caching = new Caching();

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                caching.GetOrSetObjectFromCacheAsync<string>("bad", 10, () => Task.FromException<string>(new InvalidOperationException("fail"))));

            Assert.Throws<InvalidOperationException>(() =>
                caching.GetOrSetObjectFromCache<string>("bad", 10, () => throw new InvalidOperationException("fail")));
        }

        [Fact]
        public void should_cache_empty_list_when_requested()
        {
            var caching = new Caching();

            var ret = caching.GetOrSetObjectFromCache(CacheKeyName, 20, GenerateEmptyList, cacheEmptyList: true);
            var cached = caching.GetValueFromCache<List<string>>(CacheKeyName);

            Assert.Empty(ret);
            Assert.Empty(cached);
        }

        [Fact]
        public void should_not_cache_empty_array()
        {
            var caching = new Caching();

            var ret = caching.GetOrSetObjectFromCache("arr", 20, () => Array.Empty<string>());

            Assert.Empty(ret);
            Assert.False(caching.TryGetValue<string[]>("arr", out _));
        }

        [Fact]
        public async Task should_invalidate_one_key_async_and_list_keys_async()
        {
            var caching = new Caching();
            await caching.SetValueToCacheAsync(CacheKeyName, "one", 20);
            await caching.SetValueToCacheAsync(CacheKeyName2, "two", 20);

            await caching.InvalidateAsync(CacheKeyName);

            var keys = new List<string>();
            await foreach (var key in caching.GetKeysAsync())
                keys.Add(key);

            Assert.DoesNotContain(CacheKeyName, keys);
            Assert.Contains(CacheKeyName2, keys);
            Assert.False(caching.TryGetValue<string>(CacheKeyName, out _));
        }

        [Fact]
        public void should_reject_non_positive_or_overflowing_ttl_before_factory()
        {
            var caching = new Caching();
            var calls = 0;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                caching.GetOrSetObjectFromCache("ttl", 0, () =>
                {
                    calls++;
                    return "x";
                }));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                caching.SetValueToCache("ttl", "v", int.MaxValue, CacheTimeInterval.Days));
            Assert.Equal(0, calls);
        }

        [Fact]
        public void should_not_dispose_an_injected_memory_cache()
        {
            var memory = new MemoryCache(new MemoryCacheOptions());
            var caching = new Caching(memory);
            caching.SetValueToCache("k", "v", 1, CacheTimeInterval.Minutes);

            caching.Dispose();

            Assert.Equal("v", memory.Get("k"));
            memory.Dispose();
        }

        [Fact]
        public async Task should_pass_cancellation_to_factory()
        {
            var caching = new Caching();
            using var cts = new CancellationTokenSource();
            CancellationToken seen = default;

            var value = await caching.GetOrSetObjectFromCacheAsync("token", 10, token =>
            {
                seen = token;
                return Task.FromResult("ok");
            }, cancellationToken: cts.Token);

            Assert.Equal("ok", value);
            Assert.Equal(cts.Token, seen);
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