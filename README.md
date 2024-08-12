# EasyMemoryCache
[![Build status](https://dev.azure.com/thiagoguaru/EasyMemoryCache/_apis/build/status/EasyMemoryCache%20-%20CI%20Build)](https://dev.azure.com/thiagoguaru/EasyMemoryCache/_build/latest?definitionId=24)

[![NuGet](https://buildstats.info/nuget/EasyMemoryCache)](http://www.nuget.org/packages/EasyMemoryCache)
#### .NET Component to easily implement MemoryCache (sync and async) for your .NET Core Application

# How to Use:
Open Package Manager Console and run:

```Install-Package EasyMemoryCache```

# Usage:
First, register the component in your Application:

```C#
.AddEasyCache(new CacheSettings() { ... })
```

## .NET Core Console Example

```C#
//setup our DI
var serviceProvider = new ServiceCollection()
    .AddEasyCache(new CacheSettings() { ... })
    .BuildServiceProvider();

var caching = serviceProvider.GetService<ICaching>();
return caching;
```

## ASP.NET Core Example (Startup.cs)

```C#
services.AddEasyCache(Configuration.GetSection("CacheSettings").Get<CacheSettings>());
```

### Configuration example: (Redis)
```json
"CacheSettings": {
    "CacheProvider": "Redis",
    "IsDistributed": true,
    "RedisConnectionString": "localhost:6379,password=xxx=,ssl=False,abortConnect=False"
  }
```
For MemoryCache
```json
 "IsDistributed": false,
 "CacheProvider": "MemoryCache",
```
InMemory cache will be used instead of Redis


### Then inject the interface where do you want to use, for example:
```C#
private readonly ICaching _caching;
private string UserKeyCache => "UserKey";

public UserService(ICaching caching)
{
     _caching = caching;
}
``` 
## Async:
```C#
var lstStringFromAsync = await _caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);
```

## With parameters:
```C#
var lstStringFromAsync = await _caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, () => ReturnListOfStringAsync(param));
```

## Sync:
```C#
var lstString = _caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);
```

## Check the code sample in the src directory
