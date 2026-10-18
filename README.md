<p align="center">
  <img src="https://raw.githubusercontent.com/thiagoloureiro/EasyMemoryCache/master/cacheicon.png" alt="EasyMemoryCache" width="96" />
</p>

<h1 align="center">EasyMemoryCache</h1>

<p align="center">
  One <code>ICaching</code> interface for in-memory cache, Redis, and Memcached.<br/>
  Sync and async, with dependency injection for .NET 8, 9, and 10.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/EasyMemoryCache">
    <img src="https://img.shields.io/nuget/v/EasyMemoryCache?style=flat-square&logo=nuget&logoColor=white&label=NuGet" alt="NuGet version" />
  </a>
  <a href="https://www.nuget.org/packages/EasyMemoryCache">
    <img src="https://img.shields.io/nuget/dt/EasyMemoryCache?style=flat-square&logo=nuget&logoColor=white&label=Downloads" alt="NuGet downloads" />
  </a>
  <a href="https://github.com/thiagoloureiro/EasyMemoryCache/blob/master/LICENSE">
    <img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" alt="MIT license" />
  </a>
  <img src="https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8, 9, and 10" />
  <a href="https://github.com/thiagoloureiro/EasyMemoryCache/actions/workflows/github-code-scanning/codeql">
    <img src="https://img.shields.io/github/actions/workflow/status/thiagoloureiro/EasyMemoryCache/github-code-scanning/codeql?style=flat-square&logo=github&label=CodeQL" alt="CodeQL status" />
  </a>
  <a href="https://dev.azure.com/thiagoguaru/EasyMemoryCache/_build/latest?definitionId=24">
    <img src="https://dev.azure.com/thiagoguaru/EasyMemoryCache/_apis/build/status/EasyMemoryCache%20-%20CI%20Build?style=flat-square" alt="Azure Pipelines build status" />
  </a>
</p>

## Why

Pick a provider once, then call the same methods everywhere. Switching from `IMemoryCache` to Redis or Memcached is a configuration change, not a rewrite of your services.

| | |
| --- | --- |
| **Providers** | In-memory, Redis, Memcached |
| **API** | Get-or-set, explicit get and set, invalidate one key or all, list keys |
| **Styles** | Synchronous and asynchronous |
| **TTL** | Seconds, minutes, hours, or days |
| **Redis serialization** | Newtonsoft.Json or protobuf-net |

## Install

```bash
dotnet add package EasyMemoryCache
```

Or from the Package Manager Console:

```powershell
Install-Package EasyMemoryCache
```

## Quick start

Register the cache, inject `ICaching`, and call get-or-set. The factory runs only on a miss.

### Console

```csharp
using EasyMemoryCache.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddEasyCache(new CacheSettings
    {
        CacheProvider = CacheProvider.MemoryCache
    });

var caching = services.BuildServiceProvider().GetRequiredService<ICaching>();

var users = await caching.GetOrSetObjectFromCacheAsync(
    "users",
    cacheTime: 20,
    objectSettingFunction: LoadUsersAsync);
```

### ASP.NET Core

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

A working API sample lives in [`EasyMemoryCache.SampleApi`](EasyMemoryCache.SampleApi), and a console sample lives in [`src/EasyMemoryCache.Sample`](src/EasyMemoryCache.Sample).

## Providers

`CacheProvider` selects the backend. The default is in-memory.

### In-memory

```csharp
builder.Services.AddEasyCache(new CacheSettings
{
    CacheProvider = CacheProvider.MemoryCache
});
```

```json
"CacheSettings": {
  "CacheProvider": "MemoryCache"
}
```

### Redis

```csharp
builder.Services.AddEasyCache(new CacheSettings
{
    CacheProvider = CacheProvider.Redis,
    RedisConnectionString = "localhost:6379,abortConnect=False",
    RedisSerialization = SerializationType.Protobuf
});
```

`RedisSerialization` accepts `Newtonsoft` (the default) or `Protobuf`.

```json
"CacheSettings": {
  "CacheProvider": "Redis",
  "RedisConnectionString": "localhost:6379,password=secret,ssl=False,abortConnect=False",
  "RedisSerialization": "Protobuf"
}
```

### Memcached

Pass the configuration section as the second argument so server and socket-pool settings are bound.

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
  ],
  "SocketPool": {
    "MinPoolSize": 5,
    "MaxPoolSize": 25,
    "ConnectionTimeout": "00:00:15",
    "ReceiveTimeout": "00:00:15",
    "DeadTimeout": "00:00:15",
    "QueueTimeout": "00:00:00.150"
  }
}
```

## Usage

### Get or set

The factory is skipped when the key is already cached.

```csharp
var names = _caching.GetOrSetObjectFromCache("names", 20, LoadNames);

var namesAsync = await _caching.GetOrSetObjectFromCacheAsync("names", 20, LoadNamesAsync);
```

Close over arguments when the factory needs them:

```csharp
var names = await _caching.GetOrSetObjectFromCacheAsync(
    "names",
    20,
    () => LoadNamesAsync(tenantId));
```

Empty collections are not stored unless you opt in with `cacheEmptyList: true`. The default interval is minutes. Pass `CacheTimeInterval` to use seconds, hours, or days.

```csharp
var snapshot = await _caching.GetOrSetObjectFromCacheAsync(
    "snapshot",
    cacheTime: 30,
    objectSettingFunction: LoadSnapshotAsync,
    cacheEmptyList: true,
    interval: CacheTimeInterval.Seconds);
```

### Read and write directly

```csharp
_caching.SetValueToCache("token", token, cacheTime: 2, interval: CacheTimeInterval.Hours);

var token = _caching.GetValueFromCache<string>("token");
var tokenAsync = await _caching.GetValueFromCacheAsync<string>("token");
```

### Invalidate

```csharp
_caching.Invalidate("users");
_caching.InvalidateAll();
await _caching.InvalidateAllAsync();
```

### List keys

```csharp
foreach (var key in _caching.GetKeys())
{
    Console.WriteLine(key);
}
```

## License

[MIT](LICENSE) © Thiago Loureiro
