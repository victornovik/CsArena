# CsArena.Tests

Unit-test and exploration project for C# language features, BCL APIs, concurrency primitives, and LeetCode algorithm implementations. All tests are self-contained learning artefacts — the goal is to capture *actual runtime behavior* as executable, runnable assertions.

---

## Stack

| Concern | Package / Version |
|---|---|
| Test framework | `xunit.v3` 3.2.2 |
| Parallel execution | `Meziantou.Xunit.v3.ParallelTestFramework` 1.0.6 |
| VS / Rider runner | `xunit.runner.visualstudio` 3.1.5 |
| Coverage | `coverlet.collector` 10.0.1 |
| Configuration | `Microsoft.Extensions.Configuration.*` 10.0.9 |
| Target framework | `net10.0` |
| Nullable | enabled |
| Unsafe blocks | enabled |

---

## Project layout

```
test/CsArena.Tests/
│
├── *Tests.cs                  # Feature tests (one file per topic)
├── CollectionUtil.cs          # Static test-data factory (countries, dicts)
├── HashHelpers.cs             # Hash / equality helpers shared across tests
│
├── config/
│   ├── appsettings.json       # Copied to output; base config values
│   ├── appsettings.local.json # Copied to output; local overrides
│   ├── Options.cs             # Strongly-typed config binding target
│   └── TestConfigHelper.cs   # IConfigurationRoot builder; PrintConfig helpers
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
│   ├── Car.cs                 # Has two Deconstruct overloads
│   ├── Country.cs
│   └── Player.cs              # Has indexer via PlayerList wrapper
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

`Meziantou.Xunit.v3.ParallelTestFramework` runs **every test method in parallel** — including `[Theory]` cases from the same class. xUnit v3 gives each test its own class instance, so **instance fields are safe**. What is *not* safe:

| Scope | Safe? | Notes |
|---|---|---|
| Instance fields | Yes | Fresh instance per test |
| Local variables | Yes | Stack-local |
| `static` fields on the test class | **No** | Shared across the entire AppDomain |
| OS-named mutexes / semaphores with a fixed string name | **No** | System-wide; parallel `[Theory]` cases collide |
| `Console.Out` / `Console.Error` | No | Use `ITestOutputHelper` instead |

**Rules that follow from this:**

1. Never declare `static` state on a test class unless the field is genuinely immutable (e.g. `static readonly` constants). Mutable `static` causes cross-test interference that manifests as rare, hard-to-reproduce failures.
2. When a test creates a named OS primitive (mutex, semaphore, event, pipe), use a GUID suffix so parallel theory cases don't share the same handle:
   ```csharp
   var mutexName = $"CsArena_{Guid.NewGuid():N}";
   using var mx = new Mutex(initiallyOwned: false, mutexName);
   ```
3. Always `using` OS handles — leaked mutex handles survive the test and can block the next run.

---

## Namespace and using conventions

All test classes live in `CsArena.Tests` (root topics) or `CsArena.Tests.LeetCode` (algorithms). `Xunit` is a **global implicit using** via:

```xml
<Using Include="Xunit" />   <!-- in CsArena.Tests.csproj -->
```

This means `[Fact]`, `[Theory]`, `Assert`, `ITestOutputHelper`, and `TestContext` are available in every file without an explicit `using Xunit;`. Do **not** add `using Xunit;` explicitly — it compiles but is redundant and inconsistent.

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

Each `[InlineData]` becomes an independent test case and runs in parallel with all others (including other theory cases of the same method). Keep cases independent.

### `[Fact(Skip = "reason")]` — intentional skip

Used for tests that are inherently non-deterministic or require external infrastructure:

```csharp
[Fact(Skip = "Flaky test")]
public async Task ContinueAfterAwaitInTheSameThread() { ... }

[Fact(Skip = "Integration test reaching https://google.com")]
public async Task HttpClientTest() { ... }
```

Do **not** skip a test to silence a real bug — fix the bug or delete the test.

---

## Async tests

Return `Task`; xUnit v3 awaits it correctly:

```csharp
[Fact]
public async Task SomeAsyncBehavior()
{
    await Task.Delay(50);
    Assert.True(...);
}
```

The analyzer rule `xUnit1051` advises passing `TestContext.Current.CancellationToken` to calls that accept a `CancellationToken`. This is advisory — it improves responsiveness to test cancellation but is not required. The project has `TreatWarningsAsErrors = false` so it will not break the build.

---

## Output

Use `ITestOutputHelper` (injected via constructor) to write diagnostic output. It is captured per-test and shown only on failure — no global console noise:

```csharp
public class MyTests(ITestOutputHelper output)
{
    [Fact]
    public void Something()
    {
        output.WriteLine($"Debug value: {someVar}");
        Assert.True(...);
    }
}
```

`TestConfigHelper.PrintConfig(output)` and `PrintConfigProviders(output)` use this pattern.

---

## Shared test infrastructure

### `CollectionUtil`

Provides seeded collections of `Country` objects (Russia, Norway, Finland) for collection-manipulation tests:

```csharp
var list  = CollectionUtil.CreateCountryList();           // List<Country>
var dict  = CollectionUtil.CreateCountryDictionary();     // Dictionary<string, Country> (OrdinalIgnoreCase on Code)
var sdict = CollectionUtil.CreateSortedCountryDictionary(); // SortedDictionary
var slist = CollectionUtil.CreateSortedCountryList();       // SortedList
```

### `TestConfigHelper` (config/)

Builds `IConfigurationRoot` from `appsettings.json` → `appsettings.local.json` → optional user-secrets / env-vars / command-line:

```csharp
// Minimal — JSON files only:
var cfg = TestConfigHelper.GetConfigurationRoot();

// Full stack:
var cfg = TestConfigHelper.GetConfigurationRoot(
    enableSecrets: true,
    enableEnvVar:  true,
    enableCmdLine: true,
    args: new[] { "--MyKey=value" });

// Bind to Options:
var opts = TestConfigHelper.GetOptions();   // returns Options or new Options()
```

User-secrets ID: `acab85f1-6255-410f-a9ef-b94f35eff04b` (matches `csproj`).

### Extension methods (extensions/)

| Method | Signature | Purpose |
|---|---|---|
| `MyCount` | `IEnumerable<T> → int` | Custom count (demonstrates yield iteration) |
| `Filter` | `IEnumerable<T>, Func<T,bool> → IEnumerable<T>` | Custom Where (demonstrates yield return) |
| `SelectWithIndex` | `IEnumerable<T> → IEnumerable<(T item, int index)>` | Indexed projection |
| `Random` | `() → IEnumerable<int>` | Infinite random-int stream |
| `Concat` | `string?, string? → string` | Null-safe concat (null-safe extension call demo) |
| `ReplaceAt` | `string, int, char → string` | Char replacement |
| `ParseEnum<T>` | `string → T where T : Enum` | Generic enum parse |

### Models (models/)

`Country`, `Player`, `PlayerList`, `Book`, `Car` / `CarStatistics` — plain data objects used as realistic LINQ / reflection / generics subjects. Prefer them over ad-hoc anonymous types when you need a named, reusable shape.

---

## LeetCode sub-project conventions

Each topic has two co-located files:

- **`Topic.cs`** — the algorithm implementation. Uses block-comment docstrings with problem statement, approach, and complexity.
- **`TopicTests.cs`** — the tests. One `[Fact]` or `[Theory]` per problem. Test names follow `<ProblemName>Test` / `<ProblemName>Tests`.

Implementation classes use `static` methods (pure functions); the test class has no state. Example:

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

### Case 1: New C# feature or BCL behavior

1. Find the matching `*Tests.cs` file (e.g. `AsyncTests.cs`, `LinqTests.cs`, `GenericTests.cs`). If none fits, create `NewTopicTests.cs` in the project root.
2. Add a `[Fact]` method with a descriptive name in PascalCase without redundant "Test" suffix at the topic level (the class already says "Tests").
3. Instance fields and local variables are safe. Do **not** add `static` mutable state.
4. Use `ITestOutputHelper` for diagnostic output; inject it via the constructor.

```csharp
public class NewTopicTests(ITestOutputHelper output)
{
    [Fact]
    public void SomeBehavior()
    {
        // Arrange
        var value = ...;

        // Act
        var result = ...;

        // Assert
        Assert.Equal(expected, result);
    }
}
```

### Case 2: New LeetCode problem

1. Add the algorithm to the appropriate `LeetCode/Topic.cs` static class (or create a new pair if no category fits).
2. Add a `[Fact]` or `[Theory]` to the matching `LeetCode/TopicTests.cs`.
3. For parametrized problems use `[Theory]` + `[InlineData]`; keep each case independent.
4. Name the test `<LeetCodeProblemName>Test`.

```csharp
// Sort.cs
public static int[] MySort(int[] arr) { ... }

// SortTests.cs
[Theory]
[InlineData(new[] { 3, 1, 2 }, new[] { 1, 2, 3 })]
[InlineData(new[] { 5 },       new[] { 5 })]
public void MySortTest(int[] input, int[] expected)
{
    Assert.Equal(expected, Sort.MySort(input));
}
```

### Case 3: Concurrency / OS primitives

- Any named OS resource (mutex, semaphore, pipe, event) **must** use a GUID-based name to avoid collision with parallel theory cases.
- Dispose via `using` — never let handles leak beyond the test method.
- If a test is genuinely non-deterministic (relies on scheduler timing), mark it `[Fact(Skip = "Flaky test")]` with a comment explaining why.

### Case 4: Configuration

Use `TestConfigHelper.GetConfigurationRoot(...)` to build the configuration stack. Add fixture data to `config/appsettings.local.json`; add secrets via `dotnet user-secrets set` using ID `acab85f1-6255-410f-a9ef-b94f35eff04b`.

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

---

## Running tests

```bash
# All tests
dotnet test test/CsArena.Tests/CsArena.Tests.csproj

# Verbose (show passed test names)
dotnet test test/CsArena.Tests/CsArena.Tests.csproj --logger "console;verbosity=normal"

# Single class
dotnet test test/CsArena.Tests/CsArena.Tests.csproj --filter "FullyQualifiedName~AsyncTests"

# Single method
dotnet test test/CsArena.Tests/CsArena.Tests.csproj --filter "FullyQualifiedName=CsArena.Tests.AsyncTests.InterProcessMutex"
```

---

## Checking for vulnerabilities
```bash
# Find all vulnerable packages (CVE) in the project
dotnet list package --vulnerable --include-transitive

# Find where (the whole chain) a specific package is referenced
dotnet nuget why . SQLitePCLRaw.lib.e_sqlite3
```

---

## Common pitfalls

| Pitfall | Consequence | Fix |
|---|---|---|
| `static` mutable field on test class | Cross-test pollution, rare assertion failures | Make it an instance field |
| Hard-coded OS mutex / event name | Two theory cases deadlock or corrupt shared counter | Append `Guid.NewGuid():N` to the name |
| OS handle not disposed | Handle leak, test-runner hangs on repeated runs | `using var` |
| `using Xunit;` explicit directive | Redundant (global implicit using already present) | Remove it |
| `using Xunit.Abstractions;` | Doesn't compile in xUnit v3 | `ITestOutputHelper` is now in `Xunit` namespace |
| Checking `Thread.CurrentThread` after `await` without pre-completing the task | Flaky — continuation thread is non-deterministic | Mark `[Fact(Skip = "Flaky test")]` |
