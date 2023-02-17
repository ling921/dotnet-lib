# Ling.Audit

This repository is a library includes two projects
- `Ling.Audit` provides a source generator to generate audit properties.
- `Ling.EFCore.Audit` provides a convenient audit system for `EntityFrameworkCore`.

## Getting Started

### Ling.Audit

Create your class with `partial` keyword and interface
`IHasCreationTime`, `IHasCreator<T>`, `ICreationAudited<T>`
`IHasModificationTime`, `IHasModifier<T>`, `IModificationAudited<T>`
`ISoftDelete`, `IHasDeletionTime`, `IHasDeleter<T>`, `IDeletionAudited<T>`
`IFullAudited<T>`

```csharp
public partial class Post : IFullAudited<Guid>
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    ...
}
```
The source generator will automatically generate audit properties.

### Ling.EFCore.Audit

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

### Installing

1. Package Manager
```
PM> Install-Package Ling.Audit
PM> Install-Package Ling.EFCore.Audit
```

2. .NET CLI
```
dotnet add package Ling.Audit
dotnet add package Ling.EFCore.Audit
```

## License

This project is licensed under the [Apache-2.0](LICENSE.md)
Creative Commons License - see the [LICENSE.md](LICENSE.md) file for details
