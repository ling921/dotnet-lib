### Introduction

Ling.EntityFrameworkCore.Audit is an extension library that can automatically record entity changes of `Microsoft.EntityFrameworkCore`.

### Installation

1. Package Manager
```
PM> Install-Package Ling.Audit
PM> Install-Package Ling.EntityFrameworkCore.Audit
```

2. .NET CLI
```
dotnet add package Ling.Audit
dotnet add package Ling.EntityFrameworkCore.Audit
```

### Usage

1. Add `UseAudit()` in your `DbContext` service registration code.

```csharp
// in Program.cs
builder.Services.Addxxx<xxDbContext>(
    connectionString,
    optionsAction: options => options.UseAudit());

// in Startup.cs
services.Addxxx<xxDbContext>(
    connectionString,
    optionsAction: options => options.UseAudit());
```

2. Configure audit entity by attribute or fluent api

Use `AuditIncludeAttribute` to enable auditing for entity, all properties in entity will record changes by default. Use `AuditIgnoreAttribute` on property to disable property auditing.
```csharp
[AuditInclude]
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    [AuditIgnore]
    public DateTimeOffset CreationTime { get; set; } = null!;
}
```

You can also use fluent api in `OnModelCreating`
```csharp
builder.Entity<Post>(b =>
{
    b.IsAuditable();
    b.Property(e => e.CreationTime).IsAuditable(false);
});
```
