# EasyMemoryCache

One `ICaching` interface for in-memory cache, Redis, and Memcached. Sync and async, with dependency injection for .NET 8, 9, and 10.

[![NuGet](https://img.shields.io/nuget/v/EasyMemoryCache?style=flat-square&logo=nuget&logoColor=white&label=NuGet)](https://www.nuget.org/packages/EasyMemoryCache)
[![Downloads](https://img.shields.io/nuget/dt/EasyMemoryCache?style=flat-square&logo=nuget&logoColor=white&label=Downloads)](https://www.nuget.org/packages/EasyMemoryCache)
[![License](https://img.shields.io/github/license/thiagoloureiro/EasyMemoryCache?style=flat-square)](https://github.com/thiagoloureiro/EasyMemoryCache/blob/master/LICENSE)
![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4?style=flat-square&logo=dotnet&logoColor=white)

## Install

```bash
dotnet add package EasyMemoryCache
```

## Quick start

```csharp
builder.Services.AddEasyCache(new CacheSettings
{
    CacheProvider = CacheProvider.MemoryCache
});
```

```csharp
public class UserService
{
    private readonly ICaching _caching;

    public UserService(ICaching caching) => _caching = caching;

    public Task<List<User>> GetUsersAsync() =>
        _caching.GetOrSetObjectFromCacheAsync("users", 20, LoadUsersAsync);
}
```

The factory runs only on a cache miss. Full samples are in the [repository](https://github.com/thiagoloureiro/EasyMemoryCache).

## Providers

### In-memory

```json
"CacheSettings": {
  "CacheProvider": "MemoryCache"
}
```

### Redis

`RedisSerialization` accepts `Newtonsoft` (the default) or `Protobuf`.

```csharp
builder.Services.AddEasyCache(new CacheSettings
{
    CacheProvider = CacheProvider.Redis,
    RedisConnectionString = "localhost:6379,abortConnect=False",
    RedisSerialization = SerializationType.Protobuf
});
```

### Memcached

Pass the configuration section so server and socket-pool settings are bound.

```csharp
builder.Services.AddEasyCache(
    builder.Configuration.GetSection("CacheSettings").Get<CacheSettings>(),
    builder.Configuration.GetSection("CacheSettings"));
```

```json
"CacheSettings": {
  "CacheProvider": "Memcached",
  "Servers": [
    { "Address": "localhost", "Port": 11211 }
  ]
}
```

## Usage

```csharp
var names = await _caching.GetOrSetObjectFromCacheAsync("names", 20, LoadNamesAsync);

var namesForTenant = await _caching.GetOrSetObjectFromCacheAsync(
    "names",
    20,
    () => LoadNamesAsync(tenantId));
```

Empty collections are not stored unless you opt in with `cacheEmptyList: true`. The default interval is minutes.

```csharp
var snapshot = await _caching.GetOrSetObjectFromCacheAsync(
    "snapshot",
    cacheTime: 30,
    objectSettingFunction: LoadSnapshotAsync,
    cacheEmptyList: true,
    interval: CacheTimeInterval.Seconds);
```

```csharp
_caching.SetValueToCache("token", token, cacheTime: 2, interval: CacheTimeInterval.Hours);
var token = _caching.GetValueFromCache<string>("token");

_caching.Invalidate("users");
await _caching.InvalidateAllAsync();
```

## License

[MIT](https://github.com/thiagoloureiro/EasyMemoryCache/blob/master/LICENSE)
