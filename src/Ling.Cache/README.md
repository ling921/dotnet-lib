### Introduction

`Ling.Cache` is a cache library that can easily use memory cache or redis cache.

### Installation

1. Package Manager
```
PM> Install-Package Ling.Cache
```

2. .NET CLI
```
dotnet add package Ling.Cache
```

### Usage

1. Add `Cache` in your `appsettings.json` file.

```json
{
  ...
  "Cache": {
    "Type": "Memory",
    "Configuration": "127.0.0.1:6379,password=,defaultDatabase=1",
    "InstanceName": "MyApp"
  },
  ...
}
```


2. Then add `AddDistributedCache()` in your service registration code.

```csharp
// in Program.cs
builder.Services.AddDistributedCache();

// in Startup.cs
services.AddDistributedCache(options =>
{
	options.Type = CacheType.Redis;
	options.Configuration = "127.0.0.1:6379,password=,defaultDatabase=1";
	options.InstanceName = "MyApp"
});
```
