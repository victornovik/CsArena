Write a complete, production-quality unit test for the CsArena.Tests project at senior C# level.

## What to produce

Given `$ARGUMENTS` — a topic, feature, algorithm, or scenario — write:

1. **The test method(s)** inside the correct existing class, or a new class if none fits.
2. **The implementation** if it's a LeetCode-style algorithm (in `CsArena.Core/LeetCode/Topic.cs`, test in `CsArena.Tests/LeetCode/TopicTests.cs`).
3. Where multiple cases exist, use `[Theory]` + `[InlineData]` — one case per `[InlineData]`.
4. Use `Assert.Equal(expected, actual)` — prefer typed assertions over `Assert.True(condition)`.

---

## Project facts

| Item | Value |
|---|---|
| Framework | xUnit v3 (`xunit.v3` 4.0.3), Microsoft.Testing.Platform |
| Parallel | `[assembly: Parallelization(Mode = ParallelMode.All, MaxThreads = -1)]` in `AssemblyInfo.cs` — tests run in parallel across and within classes |
| Run | `dotnet test --project CsArena/test/CsArena.Tests [--filter-class X] [--filter-method Y]` |
| Runtime | `net11.0`, C# 15, nullable enabled |
| Global using | `using Xunit;` is implicit — **never add it explicitly** |
| Test namespaces | `CsArena.Tests` (root) · `CsArena.Tests.LeetCode` (algorithm tests) |
| Code under test | Class library `CsArena/src/CsArena.Core` — namespaces `CsArena.Core.LeetCode`, `.Collections`, `.Extensions`, `.Models` |

---

## Parallelism rules — CRITICAL

Test methods run concurrently with each other, within a class and across classes; assume `[InlineData]` cases of the same `[Theory]` can also overlap.

| Situation | Rule |
|---|---|
| `static` mutable field on the test class | **Forbidden.** Use instance fields. |
| Named OS resource (Mutex, Semaphore, Pipe) | Append a GUID: `$"CsArena_{Guid.NewGuid():N}"` + `using var` |
| Sync primitive for mutual exclusion | Use `System.Threading.Lock` (C# 13) — not `object` |
| `Monitor.Wait` / `Monitor.Pulse` | Must use plain `object` — `Lock` is incompatible |
| Reentrant lock | `Lock` is reentrant — it is fine to use |
| GC / timing-sensitive asserts (`GC.GetGeneration`, elapsed time) | Sensitive to other tests allocating in parallel — keep assertions tolerant |
| Diagnostic output | `ITestOutputHelper` injected via constructor — never `Console.Write` |
| Blocking or deadlock-prone test (threads, locks, OS primitives, `Join`, `WaitOne`) | Add `Timeout = TestTimeouts.Default` to the `[Fact]`/`[Theory]` (5 s); xUnit has no global default. Pass `TestContext.Current.CancellationToken` to every call that accepts a token. |

---

## Language and API preferences (C# 13–15, .NET 9–11)

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
| `extension` indexer (C# 15) | Indexing into an existing type from an extension block (named receiver required) |
| `union` types (C# 15) | A value that is exactly one of several case types; switch without a default arm |
| `closed` class (C# 15) | Fixed set of derived types in the same assembly; exhaustive switch |
| Labeled `break` / `continue` (C# 15) | Leaving nested loops without a flag or `goto` |
| `[with(...), ..items]` (C# 15) | Passing capacity or a comparer to the collection built by a collection expression |

Not available on the current SDK: dictionary expressions (`["k": v]`) — keep `{ ["k"] = v }` initializers. Union and closed types used in a non-`file` member signature must be `internal` or `public`, not `file`.

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
| `LinqExtensions.Random()` | `CsArena.Core/Extensions/LinqExtensions.cs` | Infinite `IEnumerable<int>` random stream |
| `StringExtensions.ParseEnum<T>` | `CsArena.Core/Extensions/StringExtensions.cs` | Generic enum parse from string |

---

## LeetCode pattern

Algorithm in `CsArena.Core/LeetCode/Topic.cs` (namespace `CsArena.Core.LeetCode`) — static class, static methods, block-comment with problem, approach, complexity:

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

Test in `CsArena.Tests/LeetCode/TopicTests.cs` — start it with `using static CsArena.Core.LeetCode.Topic;` (the fully qualified name is required), then minimal, no setup, one assertion per case:

```csharp
public class ArraysTests
{
    [Theory]
    [InlineData(new[] { 1, 2, 3 }, 6)]
    public void SolveTest(int[] input, int expected)
        => Assert.Equal(expected, Arrays.Solve(input));
}
```
