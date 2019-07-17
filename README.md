# EasyMemoryCache
[![Build status](https://ci.appveyor.com/api/projects/status/leosvxv97m6cd1ki?svg=true)](https://ci.appveyor.com/project/thiagoloureiro/easymemorycache)

[![Build history](https://buildstats.info/appveyor/chart/thiagoloureiro/easymemorycache)](https://ci.appveyor.com/project/thiagoloureiro/easymemorycache/history)

[![NuGet](https://buildstats.info/nuget/EasyMemoryCache)](http://www.nuget.org/packages/EasyMemoryCache)
![](https://img.shields.io/appveyor/tests/thiagoloureiro/dapper-crud-extension.svg)
#### .NET Component to easily implement MemoryCache (sync and async) for your .NET Core Application

# How to Use:
Open Package Manager Console and run:

```Install-Package EasyMemoryCache```

# Usage:
First register the component as Singletion in your Application:

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
```services.AddSingleton<IReportService, ReportService>();```
## Async:
```var lstStringFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, ReturnListOfStringAsync);```

## With parameters:
```var lstStringFromAsync = await caching.GetOrSetObjectFromCacheAsync(CacheKeyNameForAsync, 20, () => ReturnListOfStringAsync(param));```

## Sync:
```var lstString = caching.GetOrSetObjectFromCache(CacheKeyName, 20, ReturnListOfString);```

## Check the code sample in src directory
