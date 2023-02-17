### Introduction

Ling.Audit is a source generator for audit properties.

### Installation

1. Package Manager
```
PM> Install-Package Ling.Audit
```

2. .NET CLI
```
dotnet add package Ling.Audit
```

### Usage

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
