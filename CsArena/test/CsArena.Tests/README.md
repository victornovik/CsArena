# CsArena.Tests

Unit-test and exploration project for C# language features, BCL APIs, concurrency primitives, and LeetCode algorithm implementations. Tests are self-contained learning artefacts — the goal is to capture *actual runtime behaviour* as executable, runnable assertions.

---

## Running tests

xUnit v3 4.x runs on [Microsoft.Testing.Platform](https://aka.ms/testingplatform) (MTP). The root `global.json` opts `dotnet test` into MTP mode, which is required on the .NET 10+ SDK. The project is passed with `--project`, and the VSTest-style `--filter` expression is replaced by xUnit's own `--filter-*` options.

```bash
# All tests (from the repo root)
dotnet test --project CsArena/test/CsArena.Tests

# Detailed output (show passed test names)
dotnet test --project CsArena/test/CsArena.Tests --output Detailed

# Single class
dotnet test --project CsArena/test/CsArena.Tests --filter-class "CsArena.Tests.AsyncTests"

# Single method (wildcards allowed)
dotnet test --project CsArena/test/CsArena.Tests --filter-method "*ExtensionIndexerTest"

# By trait (e.g. skip the HTTP integration tests)
dotnet test --project CsArena/test/CsArena.Tests --filter-not-trait "type=http"

# List discovered tests without running them
dotnet test --project CsArena/test/CsArena.Tests --list-tests
```

---

## Checking for vulnerabilities

```bash
# All vulnerable packages (CVE) including transitive
dotnet list package --vulnerable --include-transitive

# Full dependency chain for a specific package
dotnet nuget why . SQLitePCLRaw.lib.e_sqlite3
```

---

## Stack

| Concern | Package / Version |
|---|---|
| Test framework | `xunit.v3` 4.0.3 (runs on Microsoft.Testing.Platform) |
| Parallel execution | Native `[assembly: Parallelization(...)]` in `AssemblyInfo.cs` (replaces `Meziantou.Xunit.v3.ParallelTestFramework`, which does not support xunit 4) |
| VS / Rider runner | `xunit.runner.visualstudio` 4.0.1 |
| Test SDK | `Microsoft.NET.Test.Sdk` 18.6.0 |
| Coverage | `coverlet.collector` 10.0.1 |
| Configuration | `Microsoft.Extensions.Configuration.*` 11.0.0 |
| Target framework | `net11.0` |
| Language | C# 15 (`LangVersion=latest`) |
| Build settings | Root `Directory.Build.props`: `TreatWarningsAsErrors`, `Nullable`, `LangVersion` for all projects; the test project adds `NoWarn=NETSDK1233` in its own `Directory.Build.props` |

---

## Project layout

```
test/CsArena.Tests/
│
├── AssemblyInfo.cs            # Assembly-wide [Parallelization] setting
├── *Tests.cs                  # Feature tests (one file per topic; CSharp14Tests, CSharp15Tests for language features)
├── CollectionUtil.cs          # Static test-data factory (countries, dicts)
├── HashHelpers.cs             # Hash / equality helpers shared across tests
│
├── config/
│   ├── appsettings.json       # Copied to output; base config values
│   ├── appsettings.local.json # Copied to output; local overrides
│   ├── Options.cs             # Strongly-typed config binding target
│   └── TestConfigHelper.cs    # IConfigurationRoot builder; PrintConfig helpers
│
├── collections/
│   ├── SkipListCollection.cs
│   ├── SkipListDictionary.cs
│   ├── SkipListSetNode.cs
│   └── ThrowHelper.cs
│
├── extensions/
│   ├── IntExtensions.cs
│   ├── LinqExtensions.cs      # MyCount, Filter, SelectWithIndex, Random()
│   └── StringExtensions.cs   # Concat, ReplaceAt, ParseEnum<T>
│
├── models/
│   ├── Book.cs
│   ├── Car.cs                 # Two Deconstruct overloads
│   ├── Country.cs
│   └── Player.cs              # Indexer via PlayerList wrapper
│
└── LeetCode/
    ├── Arrays.cs + ArraysTests.cs
    ├── BinarySearch.cs + BinarySearchTests.cs
    ├── Backtracking.cs + BacktrackingTests.cs
    ├── Bits.cs + BitsTests.cs
    ├── Combinatorics.cs + CombinatoricsTests.cs
    ├── Heap.cs + HeapTests.cs
    ├── Intervals.cs + IntervalsTests.cs
    ├── Lists.cs + ListsTests.cs   (ListNode.cs — linked-list node)
    ├── Maths.cs + MathsTests.cs
    ├── Sort.cs + SortTests.cs
    ├── Stacks.cs + StacksTests.cs (StackWithMin.cs — helper)
    ├── Strings.cs + StringsTests.cs
    ├── Trees.cs + TreesTests.cs   (TreeNode.cs — binary-tree node)
    └── Trie.cs + TriesTests.cs
```

---

## Parallelism model

`AssemblyInfo.cs` sets:

```csharp
[assembly: Parallelization(Mode = ParallelMode.All, MaxThreads = -1)]
```

`ParallelMode.All` runs tests concurrently across classes **and** inside the same class (the xUnit default, `Collections`, only parallelizes across classes). `MaxThreads = -1` removes the thread cap. The old `[CollectionBehavior(DisableTestParallelization / MaxParallelThreads)]` properties are obsolete in xUnit 4 and fail the build. Keep `[Theory]` cases independent, since they can run concurrently as well. xUnit v3 creates a fresh class instance per test, so instance fields are safe. What is not safe:

| Scope | Safe? | Notes |
|---|---|---|
| Instance fields | Yes | Fresh instance per test |
| Local variables | Yes | Stack-local |
| `static` mutable fields on a test class | **No** | Shared across all parallel instances in the AppDomain |
| Named OS resources with a fixed string name | **No** | System-wide; parallel `[Theory]` cases collide |
| `Console.Out` / `Console.Error` | No | Use `ITestOutputHelper` instead |

### Rules

1. **No `static` mutable state on test classes.** Use instance fields. The symptom is intermittent assertion failures that disappear when tests run sequentially.

2. **Name OS primitives with a GUID suffix.** Every `Mutex`, `Semaphore`, `EventWaitHandle`, or named pipe must get a unique name per invocation:
   ```csharp
   var name = $"CsArena_{Guid.NewGuid():N}";
   using var mx = new Mutex(initiallyOwned: false, name);
   ```

3. **Dispose OS handles** with `using var` — leaked handles survive the test and can deadlock subsequent runs.

4. **Use `System.Threading.Lock` for new sync primitives (C# 13/.NET 9).** Generates leaner code than `lock (object)`, and it is reentrant (`AsyncTests.ReentrantLockTest` re-enters one). Plain `object` is required only for:
   - `Monitor.Wait` / `Monitor.Pulse` / `Monitor.PulseAll` — these APIs do not work with `Lock` (see `AsyncTests.MonitorWaitTest`)

5. **Spin-wait instead of `Thread.Sleep` to guarantee task completion before `await`.** Sleep introduces timer-resolution races under load:
   ```csharp
   while (!task.IsCompleted) Thread.Yield();
   await task; // continuation runs inline on this thread
   ```

6. **Replace fixed delays for multi-task synchronisation with `WhenAll`.** `await Task.Delay(500)` to "wait for background tasks" is a timing race. Capture the tasks and `await Task.WhenAll(task1!, task2!)`.

7. **Don't block the thread pool inside sync event handlers.** `Task.Delay(n).Wait()` consumes a thread-pool thread for the timer callback; under load this can delay the handler past a racing async continuation. Use `Thread.Sleep(n)` or remove the delay.

---

## Namespace and using conventions

All test classes live in `CsArena.Tests` (root topics) or `CsArena.Tests.LeetCode` (algorithms). `Xunit` is a **global implicit using** via:

```xml
<Using Include="Xunit" />   <!-- CsArena.Tests.csproj -->
```

`[Fact]`, `[Theory]`, `Assert`, `ITestOutputHelper`, and `TestContext` are available everywhere. **Never add `using Xunit;` explicitly** — it compiles but is redundant. `using Xunit.Abstractions;` does not exist in xUnit v3; `ITestOutputHelper` moved to the `Xunit` namespace.

Add explicit `using` only for non-Xunit namespaces your file needs:

```csharp
using System.Collections.Concurrent;
using CsArena.Tests.extensions;
using CsArena.Tests.models;
```

---

## Test attributes

### `[Fact]` — single-case test

```csharp
[Fact]
public void SomeLanguageFeature()
{
    var result = ...;
    Assert.Equal(expected, result);
}
```

### `[Theory]` + `[InlineData]` — parameterized test

```csharp
[Theory]
[InlineData(5,  5)]
[InlineData(10, 10)]
public void MyAlgorithm(int input, int expected)
{
    Assert.Equal(expected, Compute(input));
}
```

Each `[InlineData]` is an independent parallel test. Keep cases fully independent.

### `[Fact(Skip = "reason")]` — intentional skip

Only for:
- HTTP integration tests reaching external URLs: `Skip = "Integration test reaching https://..."`
- Genuinely non-deterministic scheduler-dependent behaviour after all determinism fixes have been exhausted

---

## Async tests

Return `Task`; xUnit v3 awaits it:

```csharp
[Fact]
public async Task SomeAsyncBehaviour()
{
    await Task.Delay(50);
    Assert.True(...);
}
```

The analyzer rule `xUnit1051` advises passing `TestContext.Current.CancellationToken` to calls that accept a `CancellationToken`. Advisory only — not required, won't break the build.

### `Task.WhenEach` (.NET 9+)

Returns `IAsyncEnumerable<Task<T>>` yielding each task as it completes — in completion order, not declaration order:

```csharp
await foreach (var task in Task.WhenEach(t1, t2, t3))
    results.Add(await task);
```

---

## C# 13–15 and .NET 9–11 features in use

| Feature | Where |
|---|---|
| `System.Threading.Lock` | `AsyncTests.AttachedToParentTasks`, `AsyncTests.WaitOnAsyncVoid`, `AsyncTests.ReentrantLockTest` |
| `field` keyword (semi-auto property) | `CSharp14Tests.FieldKeywordTest` |
| `params ReadOnlySpan<T>` | `CSharp14Tests.ParamsSpanTest` |
| Extension blocks (`extension`) | `CSharp14Tests.ExtensionBlockTest`, `CSharp14Feature` |
| Null-conditional assignment (`x?.Prop = v`) | `CSharp14Tests.NullConditionalAssignmentTest` |
| `Enumerable.Index()` (.NET 9) | `LinqTests.IndexOperator` |
| `Enumerable.CountBy()` (.NET 9) | `LinqTests.CountByKey` |
| `Enumerable.AggregateBy()` (.NET 9) | `LinqTests.AggregateByKey` |
| `OrderedDictionary<K,V>` (.NET 9) | `CollectionTests.OrderedDictionary` |
| Union types (`union Shape(Circle, Rect, Triangle)`) | `CSharp15Tests.UnionTypeExhaustiveSwitchTest`, `UnionTypePatternMatchingTest` |
| Closed hierarchies (`closed` class) | `CSharp15Tests.ClosedHierarchyExhaustiveSwitchTest` |
| Extension indexers | `CSharp15Tests.ExtensionIndexerTest` |
| Labeled `break` / `continue` | `CSharp15Tests.LabeledBreakTest`, `LabeledContinueTest` |
| Collection expression arguments (`[with(...), ..x]`) | `CSharp15Tests.CollectionExpressionArgumentsTest` |
| Static members on interfaces (older runtimes) | `CSharp15Tests.StaticInterfaceMemberTest` |

Not available yet (checked on SDK 11.0.100): dictionary expressions (`["a": 1]`) — dictionary initializers stay as `{ ["a"] = 1 }`. The memory-safety preview needs `LangVersion=preview` and is not used.
---

## Output

Use `ITestOutputHelper` (injected via constructor) — captured per-test, shown only on failure:

```csharp
public class MyTests(ITestOutputHelper output)
{
    [Fact]
    public void Something()
    {
        output.WriteLine($"debug: {someVar}");
        Assert.Equal(expected, actual);
    }
}
```

`TestConfigHelper.PrintConfig(output)` and `PrintConfigProviders(output)` use this pattern.

---

## Shared test infrastructure

### `CollectionUtil`

Seeded collections of `Country` objects (Russia, Norway, Finland):

```csharp
CollectionUtil.CreateCountryList()             // List<Country>
CollectionUtil.CreateCountryDictionary()       // Dictionary<string, Country> (OrdinalIgnoreCase on Code)
CollectionUtil.CreateSortedCountryDictionary() // SortedDictionary<string, Country>
CollectionUtil.CreateSortedCountryList()       // SortedList<string, Country>
```

### `TestConfigHelper` (config/)

Builds `IConfigurationRoot` from `appsettings.json` → `appsettings.local.json` → optional user-secrets / env-vars / command-line:

```csharp
// JSON files only:
var cfg = TestConfigHelper.GetConfigurationRoot();

// Full stack:
var cfg = TestConfigHelper.GetConfigurationRoot(
    enableSecrets: true,
    enableEnvVar:  true,
    enableCmdLine: true,
    args: new[] { "--MyKey=value" });

// Bind to Options:
var opts = TestConfigHelper.GetOptions(); // returns Options or new Options()
```

User-secrets ID: `acab85f1-6255-410f-a9ef-b94f35eff04b` (matches `csproj`).

### Models (models/)

`Country`, `Player` / `PlayerList`, `Book`, `Car` / `CarStatistics` — plain data objects used as realistic LINQ / reflection / generics subjects. Prefer them over ad-hoc anonymous types when you need a named, reusable shape.

---

## LeetCode sub-project conventions

Each topic has two co-located files:

- **`Topic.cs`** — static class, static methods, block-comment docstring with problem statement, approach, and complexity.
- **`TopicTests.cs`** — one `[Fact]` or `[Theory]` per problem, named `<ProblemName>Test`.

```csharp
// Sort.cs
public static class Sort
{
    public static void QuickSort(int[] arr) { ... }
}

// SortTests.cs
public class SortTests
{
    [Fact]
    public void QuickSortArrayTest()
    {
        int[] arr = [5, 3, 1, 4, 2];
        Sort.QuickSort(arr);
        Assert.Equal([1, 2, 3, 4, 5], arr);
    }
}
```

---

## How to add a new test

### Case 1: New C# feature or BCL behaviour

Find the matching `*Tests.cs` or create one. Add a `[Fact]`. No `static` mutable state.

```csharp
public class NewTopicTests(ITestOutputHelper output)
{
    [Fact]
    public void SomeBehaviour()
    {
        var result = ...;
        Assert.Equal(expected, result);
    }
}
```

### Case 2: New LeetCode problem

Add implementation to `LeetCode/Topic.cs`, test to `LeetCode/TopicTests.cs`. Use `[Theory]` + `[InlineData]` for parameterized cases.

```csharp
[Theory]
[InlineData(new[] { 3, 1, 2 }, new[] { 1, 2, 3 })]
[InlineData(new[] { 5 },       new[] { 5 })]
public void MySortTest(int[] input, int[] expected)
{
    Assert.Equal(expected, Sort.MySort(input));
}
```

### Case 3: Concurrency / OS primitives

GUID-suffix every named resource; dispose with `using var`:

```csharp
var name = $"CsArena_{Guid.NewGuid():N}";
using var mx = new Mutex(initiallyOwned: false, name);
```

Use `System.Threading.Lock` (not `object`) for mutual-exclusion-only locks.

### Case 4: Configuration

```csharp
public class NewConfigTests(ITestOutputHelper output)
{
    [Fact]
    public void MyOptionIsReadCorrectly()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot();
        Assert.Equal("expected", cfg["MySection:MyKey"]);
    }
}
```

Add fixture data to `config/appsettings.local.json`; add secrets via:
```bash
dotnet user-secrets set "MySection:MyKey" "value" --id acab85f1-6255-410f-a9ef-b94f35eff04b
```

---

## Skill

`/unit-test-writer <topic>` — invokes `.claude/commands/unit-test-writer.md`. Scaffolds a complete, senior-level C# test with all parallelism rules, C# 15 feature preferences, and correct xUnit v3 (4.x) conventions.

---

## Pitfalls and their fixes

| Pitfall | Consequence | Fix |
|---|---|---|
| `static` mutable field on test class | Intermittent cross-test pollution (`AsyncTests.result`, `HttpTests.TotalPageSizes` — both fixed) | Instance field |
| Named OS mutex with a fixed string | Parallel theory cases deadlock or corrupt a shared counter (was `"UniqueMutexName"` — fixed) | `$"CsArena_{Guid.NewGuid():N}"` |
| OS handle not disposed | Handle leak; runner hangs on repeated runs | `using var` |
| `lock (new object())` for mutual exclusion | Correct but suboptimal; misses C# 13 `Lock` | `var sync = new Lock()` (except when using `Monitor.Wait` / `Pulse`) |
| `Meziantou.Xunit.v3.ParallelTestFramework` with xunit 4 | Runner crashes with "Sequence contains no elements" and runs no tests | Remove it; use `[assembly: Parallelization(Mode = ParallelMode.All)]` |
| `dotnet test <path>` / `--filter` on the .NET 10+ SDK | "Testing with VSTest target is no longer supported" error | Use `dotnet test --project <path>` and `--filter-class` / `--filter-method` / `--filter-trait` |
| `[CollectionBehavior(MaxParallelThreads = ...)]` | CS0619 (obsolete) in xUnit 4 | `[assembly: Parallelization(MaxThreads = ...)]` |
| `Task.Delay(n).Wait()` in a sync event handler | Thread-pool pressure delays handler past a racing async continuation (was in `WaitOnAsyncVoid` — fixed) | `Thread.Sleep(n)` or remove |
| `Thread.Sleep(n)` to guarantee task completion before `await` | Timer-resolution race under load (was in `ContinueAfterAwaitInTheSameThread` — fixed) | `while (!task.IsCompleted) Thread.Yield()` |
| Fixed `await Task.Delay(n)` to wait for background tasks | Race when system is slow (was `await Task.Delay(500)` in `AttachedToParentTasks` — fixed) | `await Task.WhenAll(task1!, task2!)` |
| `using Xunit;` explicit directive | Redundant — global implicit using already present | Remove |
| `using Xunit.Abstractions;` | Does not compile in xUnit v3 | Remove — `ITestOutputHelper` is now in `Xunit` namespace |

---

## Static fields that are intentionally `static`

These look like pitfalls but are not — they are the *subject* of the test:

| Field | Class | Why it must be static |
|---|---|---|
| `TestStatic.TestValue` in `LanguageTests` | Nested helper | Demonstrates static constructors run once, before instance constructors |
| `Item<T>.InstanceCount` in `GenericTests` | Nested helper | Demonstrates generic type specialization (each closed type has its own static) |
| `Outer<T>.Inner<U,V>.StaticCtorCount` in `GenericTests` | Nested helper | Demonstrates static constructor runs once per closed generic type |
| `HttpTests.HttpClientt`, `HttpTests.Urls` | Test class | `static readonly` — `HttpClient` should be reused; `Urls` is immutable |
