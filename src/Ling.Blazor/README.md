# Ling.Blazor


## Introduction

`Ling.Blazor` is a library of Blazor.


## Installation

1. Package Manager
```
PM> Install-Package Ling.Blazor
```

2. .NET CLI
```
dotnet add package Ling.Blazor
```


## Usage

1. Condition

Display content only when condition is `true`

```razor
@using Ling.Blazor.Components;

<Condition Predicate="@condition">
    // display when condition is true
</Condition>
```

Display content both when condition is `true` and `false`

```razor
@using Ling.Blazor.Components;

<Condition Predicate="@condition">
    <True>
        // display when condition is true
    </True>
    <False>
        // display when condition is false
    </False>
</Condition>
```

2. ForEach

```razor
@using Ling.Blazor.Components;

<ForEach Source="@list">
    <Template Context="item">
        // display each with @item
    </Template>
    <Separator>
        // display separator between items, optional
    </Separator>
    <NoContent>
        // display when list is null or empty, optional
    </NoContent>
</ForEach>
```

3. Switch

```razor
@using Ling.Blazor.Components;

<Switch Value="@value">
    <Case When="1">
        // display when value is 1
    </Case>
    <Case When="2">
        // display when value is 2
    </Case>

    ...

    <Default>
        // display when value not matched
    </Default>
</Switch>
```

> Note: Be careful when using `Switch` with string value, if value is number, you should use like `<Case When="@("1")">`


# License

This project is licensed under the Apache 2.0 license
