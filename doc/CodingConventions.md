# Coding Conventions

This document describes the coding conventions used in the Voxura project.

## C# Coding Conventions

The  C# coding conventions used in the Voxura project are based on the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions). 

We have the following additions and preferences:

### One line statements
For one line statements, the opening and closing braces can be omitted. So, both of these examples are valid:

```csharp
if (condition) 
    DoSomething();

for (int i = 0; i < 10; i++) 
    DoSomething();
```

```csharp
if (condition) 
{
    DoSomething();
}

for (int i = 0; i < 10; i++) 
{
    DoSomething();
}
```

### Parentheses, operators
Parantheses should have a space before and after the opening and closing parantheses. Operators should have a space before and after the operator. 

```csharp
if (condition) 
{
    DoSomething();
}

for (int i = 0; i < 10; i++) 
{
    DoSomething();
}
```

### Language
All variable names, method names, class names, comments, etc. should be in English.

### Comments
All `public` or `protected` methods, classes, fields, properties MUST have XML comments. Commenting private methods is not required, but nice to have.



### Case Conventions
- Class names should be in PascalCase
- Method / Function names should be in PascalCase, and have verbs
- Variable names should be in camelCase
- Constants should be in UPPERCASE
- Properties should be in PascalCase
- Fields should be in _camelCase, with a leading underscore


### If - return
If the `if` statement is used to return a value, the `return` statement should be on the next line or with brackets. 

```csharp
if (condition)
   return true;

if (condition)
{
    return true;
}
```

### Regions
Don't use regions to separate private / protected / public methods. 

### Properties
You may use backing fields and properties next to each other.

```csharp

public....

private int _myField;
public int MyProperty { get { return _myField; } set { _myField = value; } }

public int Property2 { get; set; }

private int _countFoo = 0; // doesn't have to be moved to the top of the file
public int Foo()
{
    _countFoo++;
    return _countFoo;
}
```

### LINQ
Prefer using methods instead of query syntax. 

```csharp
var result = list.Where(x => x > 5).Select(x => x * 2).ToList();
```

instead of:

```csharp
var result = (from x in list where x > 5 select x * 2).ToList();
```
### Async / Await
Use async / await for asynchronous operations. Asynchronous methods should have the "Async" suffix.

```csharp
public async Task<int> DoSomethingAsync()
{
    Task.Delay(1000);
    return _counter++;
}
```

### C# Language Features
Feel free to use any C# language feature, as long as it helps readability and maintainability.

### Unit Tests
Unit tests should be written in a separate project, and should be named `ProjectName.Tests`.

Use `Nunit` for unit tests.

Use Shouldly for assertions.

```csharp
result.ShouldBe(5);
```

For mocking, use NSubstitute.


