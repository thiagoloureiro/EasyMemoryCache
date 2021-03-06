# EasyMemoryCache
[![Build status](https://ci.appveyor.com/api/projects/status/leosvxv97m6cd1ki?svg=true)](https://ci.appveyor.com/project/thiagoloureiro/easymemorycache)

[![Build history](https://buildstats.info/appveyor/chart/thiagoloureiro/easymemorycache)](https://ci.appveyor.com/project/thiagoloureiro/easymemorycache/history)

[![NuGet](https://buildstats.info/nuget/EasyMemoryCache)](http://www.nuget.org/packages/EasyMemoryCache)
![](https://img.shields.io/appveyor/tests/thiagoloureiro/easymemorycache.svg)
#### .NET Component to easily implement MemoryCache (sync and async) for your .NET Core Application

# How to Use:
Open Package Manager Console and run:

```Install-Package EasyMemoryCache```

# Usage:
First register the component as Singleton in your Application:

```.AddSingleton<ICaching, Caching>()```

## .NET Core Console Example
```//setup our DI
var serviceProvider = new ServiceCollection()
    .AddSingleton<ICaching, Caching>()
    .BuildServiceProvider();

var caching = serviceProvider.GetService<ICaching>();
return caching;
```

## ASP.NET Core Example (Startup.cs)
```services.AddSingleton<ICaching, Caching>();```
### Then inject the interface where do you want to use, example:
```
private readonly ICaching _caching;
private string UserKeyCache => "UserKey";

public UserService(ICaching caching)
{
     _caching = caching;
}
``` 
## Async:
```var lstStringFromAsync = await _caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);```

## With parameters:
```var lstStringFromAsync = await _caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, () => ReturnListOfStringAsync(param));```

## Sync:
```var lstString = _caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);```

## Check the code sample in src directory
