Write a complete, production-quality unit test for the CsArena.Tests project at senior C# level.

## What to produce

Given `$ARGUMENTS` — a topic, feature, algorithm, or scenario — write:

1. **The test method(s)** inside the correct existing class, or a new class if none fits.
2. **The implementation** if it's a LeetCode-style algorithm (in `LeetCode/Topic.cs`, test in `LeetCode/TopicTests.cs`).
3. Where multiple cases exist, use `[Theory]` + `[InlineData]` — one case per `[InlineData]`.
4. Use `Assert.Equal(expected, actual)` — prefer typed assertions over `Assert.True(condition)`.

---

## Project facts

| Item | Value |
|---|---|
| Framework | xUnit v3 (`xunit.v3` 3.2.2) |
| Parallel | `Meziantou.Xunit.v3.ParallelTestFramework` — every test runs in parallel |
| Runtime | `net10.0`, C# 14, nullable enabled |
| Global using | `using Xunit;` is implicit — **never add it explicitly** |
| Test namespaces | `CsArena.Tests` (root) · `CsArena.Tests.LeetCode` (algorithms) |

---

## Parallelism rules — CRITICAL

Every test method runs concurrently with all others in the same AppDomain, including multiple `[InlineData]` cases of the same `[Theory]`.

| Situation | Rule |
|---|---|
| `static` mutable field on the test class | **Forbidden.** Use instance fields. |
| Named OS resource (Mutex, Semaphore, Pipe) | Append a GUID: `$"CsArena_{Guid.NewGuid():N}"` + `using var` |
| Sync primitive for mutual exclusion | Use `System.Threading.Lock` (C# 13) — not `object` |
| `Monitor.Wait` / `Monitor.Pulse` | Must use plain `object` — `Lock` is incompatible |
| Reentrant lock | Must use plain `object` — `Lock` does not support reentrancy |
| Diagnostic output | `ITestOutputHelper` injected via constructor — never `Console.Write` |

---

## Language and API preferences (C# 13/14, .NET 9/10)

Use modern features where they fit naturally — don't force them:

| Feature | When to use |
|---|---|
| `System.Threading.Lock` | Any new sync primitive for mutual exclusion |
| Collection expressions `[a, b, c]` | Array/list literals in arrange and assert |
| `Task.WhenEach` | Consuming tasks in completion order |
| `Enumerable.Index()` | Enumerate with (index, item) pairs |
| `Enumerable.CountBy(key)` | Frequency maps without GroupBy |
| `Enumerable.AggregateBy(key, seed, fn)` | Per-group fold without GroupBy + Aggregate |
| `OrderedDictionary<K,V>` | Insertion-ordered + indexed dictionary |
| `params ReadOnlySpan<T>` | Variadic helpers with zero heap pressure |
| `field` keyword | Semi-auto properties with validation in one accessor |
| `extension` block | Extending numeric or domain types |

---

## Test structure templates

### Single fact

```csharp
[Fact]
public void DescriptiveName()
{
    // Arrange
    var input = ...;

    // Act
    var result = DoSomething(input);

    // Assert
    Assert.Equal(expected, result);
}
```

### Parameterized theory

```csharp
[Theory]
[InlineData(arg1, expected1)]
[InlineData(arg2, expected2)]
public void DescriptiveName(int input, int expected)
{
    Assert.Equal(expected, Algorithm.Solve(input));
}
```

### Async test

```csharp
[Fact]
public async Task DescriptiveName()
{
    var result = await SomeAsyncOperation();
    Assert.Equal(expected, result);
}
```

### Test with output

```csharp
public class MyTests(ITestOutputHelper output)
{
    [Fact]
    public void DescriptiveName()
    {
        output.WriteLine($"debug: {value}");
        Assert.Equal(expected, actual);
    }
}
```

### Concurrency test with OS mutex

```csharp
[Theory]
[InlineData(5)]
[InlineData(10)]
public void DescriptiveName(int threadCount)
{
    var name = $"CsArena_{Guid.NewGuid():N}";
    var sum = 0;
    var threads = Enumerable.Range(0, threadCount).Select(_ => new Thread(() =>
    {
        using var mx = new Mutex(false, name);
        mx.WaitOne();
        sum++;
        mx.ReleaseMutex();
    })).ToList();

    threads.ForEach(t => t.Start());
    threads.ForEach(t => t.Join());

    Assert.Equal(threadCount, sum);
}
```

---

## Skip policy

`[Fact(Skip = "reason")]` is only for:

- Integration tests hitting external URLs — reason: `"Integration test reaching https://..."`
- Tests that are **genuinely** non-deterministic (scheduler-dependent) — first try to eliminate the non-determinism:
  - Replace fixed `Thread.Sleep` delays with `while (!task.IsCompleted) Thread.Yield()`
  - Replace `Task.Delay(n).Wait()` (thread-pool pressure) with `Thread.Sleep(n)`
  - Replace fixed polling timeouts with `await Task.WhenAll(...)`

---

## Shared helpers

| Helper | Location | Purpose |
|---|---|---|
| `CollectionUtil.CreateCountryList()` | `CollectionUtil.cs` | 3-country `List<Country>` seed data |
| `CollectionUtil.CreateCountryDictionary()` | `CollectionUtil.cs` | `Dictionary<string, Country>` (OrdinalIgnoreCase on Code) |
| `TestConfigHelper.GetConfigurationRoot(...)` | `config/TestConfigHelper.cs` | Builds `IConfigurationRoot` with optional secrets/env/cmdline |
| `LinqExtensions.Random()` | `extensions/LinqExtensions.cs` | Infinite `IEnumerable<int>` random stream |
| `StringExtensions.ParseEnum<T>` | `extensions/StringExtensions.cs` | Generic enum parse from string |

---

## LeetCode pattern

Algorithm in `LeetCode/Topic.cs` — static class, static methods, block-comment with problem, approach, complexity:

```csharp
public static class Arrays
{
    /**
        Problem statement...
        # Approach
            ...
        # Complexity
            - Time: O(N)
            - Space: O(1)
    */
    public static int Solve(int[] nums) { ... }
}
```

Test in `LeetCode/TopicTests.cs` — minimal, no setup, one assertion per case:

```csharp
public class ArraysTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 6)]
    public void SolveTest(int[] input, int expected)
        => Assert.Equal(expected, Arrays.Solve(input));
}
```
