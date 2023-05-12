### Introduction

`Ling.Blazor` is a library for `Blazor`.

### Installation

1. Package Manager
```
PM> Install-Package Ling.Blazor
```

2. .NET CLI
```
dotnet add package Ling.Blazor
```

### Usage

1. ConditionView
```razor
@using Ling.Blazor.Components;

<ConditionView Predicate="@condition">
    <TrueContent>
        // display when condition is true
    </TrueContent>
    <FalseContent>
        // display when condition is false
    </FalseContent>
</ConditionView>
```
or 
```razor
@using Ling.Blazor.Components;

<ConditionView Predicate="@condition">
    // display when condition is true
</ConditionView>
```

2. EnumerationView
```razor
@using Ling.Blazor.Components;

<EnumerationView Source="@list">
    <EachTemplate Context="item">
        // display each with @item
    </EachTemplate>
    <SeparatorContent>
        // display separator between items, optional
    </SeparatorContent>
    <EmptyContent>
        // display when list is null or empty, optional, defalut is string "empty"
    </EmptyContent>
</EnumerationView>
```

3. SwitchView
```razor
@using Ling.Blazor.Components;

<SwitchView Value="@value">
    <SwitchCase When="1">
        // display when value is 1
    </SwitchCase>
    <SwitchCase When="2">
        // display when value is 2
    </SwitchCase>

    ...

    <SwitchDefault>
        // display when value not matched
    </SwitchDefault>
</SwitchView>
```