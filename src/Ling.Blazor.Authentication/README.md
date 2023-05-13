### Introduction

`Ling.Blazor.Authentication` is a library that provides JWT authentication for Blazor applications.

### Installation

1. Package Manager

You can install this library using either Package Manager or .NET CLI.

```
PM> Install-Package Ling.Blazor.Authentication
```

2. .NET CLI
```
dotnet add package Ling.Blazor.Authentication
```

### Usage

To use this library, you need to follow these steps:

1. Implement your own `JwtAuthenticationService` class that inherits from `JwtAuthenticationServiceBase` class. For example:
```csharp
public class JwtAuthenticationService : JwtAuthenticationServiceBase
{
    public AuthenticationService(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public Task LoginAsync(string username, string password, bool isPersistent = false, CancellationToken cancellationToken = default)
    {
       // Write your login logic here and get the tokenInfo object

       // Remember to call 'SetTokenAsync' to save the tokenInfo
       await SetTokenAsync(tokenInfo, cancellationToken);
    }
}
```

2. Register the services in `Program.cs` file. For example:
```csharp
builder.Services.AddJwtAuthorization<JwtAuthenticationService>(); // You can configure 'JwtAuthOptions' here if needed

builder.Services.AddServerAPI(baseAddress); // Replace baseAddress with your actual API server address
```

The `AddServerAPI` method will register an `HttpClient` for you, which will automatically set the `Authorization` header for your requests.
