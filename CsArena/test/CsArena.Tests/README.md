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

# By category (see "Test categories"): only GC tests, or everything except the concurrency ones
dotnet test --project CsArena/test/CsArena.Tests --filter-trait "category=gc"
dotnet test --project CsArena/test/CsArena.Tests --filter-not-trait "category=concurrency"

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

## Timeouts and runner settings

| Setting | Where | Effect |
|---|---|---|
| `[Fact(Timeout = TestTimeouts.Default)]` (5 s) | every test in `AsyncTests` | A test that blocks or deadlocks fails with "Test execution timed out" and the run continues. Works for async tests and for synchronous ones (blocked on a lock, `Join`, `WaitOne`, ...); the stuck thread is left behind. |
| `--timeout 10m` | CI `dotnet test` step | Aborts the whole test session (exit code 3) if it takes longer, as a backstop for hangs outside a timed test. |
| `--show-slowest-tests 10` | CI `dotnet test` step | Lists the slowest tests in the run summary. |
| `xunit.runner.json` | test project, copied to the output folder | `culture: invariant` (results do not depend on the machine's locale) and `shutdownForegroundThreadWaitSeconds: 10` (a test that leaves a foreground thread running cannot keep the process alive for more than 10 s after the tests finish). |

Parallelism is configured in one place only: `[assembly: Parallelization(...)]` in `AssemblyInfo.cs` (see the parallelism section); `xunit.runner.json` deliberately has no parallelism keys. xUnit has no global per-test default timeout, so a new test that blocks threads or waits on OS primitives should carry `Timeout = TestTimeouts.Default` itself.

xUnit analyzer rule xUnit1069 asks every test with a `Timeout` to reference `TestContext.Current.CancellationToken`. `AsyncTests` blocks on threads, locks and OS mutexes that cannot observe a token, so `.editorconfig` lowers that rule to a suggestion for that one file; elsewhere, pass the token to the calls that accept it.

Locally, the same flags work on any run, for example `dotnet test --solution CsArena.slnx --timeout 5m --show-slowest-tests 5`.

---

## Keeping tests fast

The slowest test in the suite takes about 150 ms. The six tests that used to be tagged `slow` (0.2 to 1 s each) were sped up without changing what they check:

| Test | Was | Now |
|---|---|---|
| `ConcurrentBag` (2 rows) | `while (!bag.IsEmpty)` spawned a consumer task per spin: 4,115 tasks for 10 items on the first run | One consumer per item, in rounds until the bag is empty (normally one round) |
| `ThreadLocal` | `Thread.Sleep(1..4 ms)` in each of 100 iterations (about 180 to 300 ms) | Deterministic `Thread.SpinWait` work: the iterations still overlap on 5 to 11 threads, in about 10 ms |
| `AsyncPuzzler` | `Task.Delay(500)` | The async method awaits a gate (`TaskCompletionSource`) that the test opens from another thread; it also asserts that the task is incomplete and, after `.Result`, that `result` was set |
| `AttachedToParentTasks` | Sleeps of 100/200/300 ms so the outer task finished "first" | The detached tasks wait for a gate that is opened once the outer task is recorded, so the order no longer depends on timing; the attached task sleeps 20 ms |
| `StopWatchTest` | 1000 ms real delay | 100 ms real delay: it measures real clocks, so it cannot be faked, and the 15 ms clock-resolution tolerance does not depend on the delay length |

Prefer a gate (`TaskCompletionSource`, `ManualResetEventSlim`) over a sleep or `Task.Delay` whenever a test only needs "not finished yet": it is faster and cannot flake on a slow machine. Use a real delay only when the real clock is the thing under test.

---

## Code coverage

Coverage is collected with `Microsoft.Testing.Extensions.CodeCoverage` (Cobertura output) over the code in `CsArena.Core`:

```bash
dotnet test --solution CsArena.slnx --coverage --coverage-output-format cobertura
# report: TestResults/<guid>.cobertura.xml

# Markdown summary per file, and a failing exit code below the minimum line coverage
python .github/scripts/coverage_summary.py TestResults --min-line 90
```

Current level: about 94% of lines and 93% of branches (Release). CI (`MIN_LINE_COVERAGE` in `.github/workflows/ci.yml`) fails when line coverage drops below 90%, writes the summary to the run page and uploads the Cobertura file as an artifact.

Why not `coverlet`: `coverlet.collector` is a VSTest data collector, which does nothing on Microsoft.Testing.Platform; `coverlet.MTP` ran but reported 0 lines here (before the code was moved; not retried afterwards). Neither tool reported anything for code inside the test assembly itself (the diagnostic log shows it skipped as an excluded path); once the code under test moved into its own project, both the Microsoft extension and the CI threshold worked, so that is the supported layout here.

---

## Platforms and CI

The suite is expected to pass on **Windows and Linux**. Last verified: Windows 10, and Ubuntu on WSL 2 with SDK 11.0.100 (440 passed, 3 skipped, repeated runs stable). The 3 skipped tests are the opt-in `[NetworkFact]` tests (`category=network`): CI does not set `CSARENA_NETWORK_TESTS`, so they are skipped on both operating systems.

`.github/workflows/ci.yml` runs on pushes and pull requests to `main`, daily at 12:10 MSK (09:10 UTC; GitHub cron is UTC and Moscow has no DST), and on demand (`workflow_dispatch`):

| Job | Runner | Steps |
|---|---|---|
| Build and test | `windows-latest` and `ubuntu-latest` (matrix, `fail-fast: false`) | `dotnet restore`, `dotnet build -c Release`, `dotnet test --solution CsArena.slnx -c Release --no-build --coverage --coverage-output-format cobertura`, coverage summary and threshold, artifact upload |
| Vulnerable packages | `ubuntu-latest` | `dotnet list CsArena.slnx package --vulnerable --include-transitive`; the job fails if the output says `has the following vulnerable packages` (the command itself exits 0 on findings) |

Warnings are errors (root `Directory.Build.props`), so a new compiler or analyzer warning fails the build step. The workflow has not yet run on GitHub; the same commands were run locally.

To reproduce the Linux run on Windows, use WSL with the .NET 11 SDK. Build from a copy on the Linux filesystem rather than `/mnt/d/...`, so Linux `bin/` and `obj/` do not end up in the Windows working tree (`obj/project.assets.json` holds absolute paths):

```bash
export XDG_RUNTIME_DIR="$HOME/xdg-runtime" && mkdir -p -m 700 "$XDG_RUNTIME_DIR"   # WSL only, see below
dotnet test --solution CsArena.slnx -c Release
```

WSL points `XDG_RUNTIME_DIR` at `/mnt/wslg/runtime-dir`, whose owner .NET rejects. Named mutexes (used by `AsyncTests.InterProcessMutex`) and the SDK's first-run setup then fail with `The directory '/mnt/wslg/runtime-dir' does not have the expected owner or permissions`. A private directory fixes it; plain Linux hosts and GitHub runners are not affected.

Categories (see "Test categories") behave the same on both platforms, so they are the way to run a subset in either place, for example a quick loop without the concurrency tests, or only the GC tests:

```bash
dotnet test --solution CsArena.slnx --filter-not-trait "category=concurrency"
dotnet test --solution CsArena.slnx --filter-trait "category=gc"
```

CI always runs the full suite (nothing is filtered out except the network tests, which skip themselves). To run the network tests on a machine that has internet access, set `CSARENA_NETWORK_TESTS=1` (see `[NetworkFact]`).

Platform notes for new tests:
- Don't depend on the OS line break or on the source file's line endings. Source files are LF everywhere (`.gitattributes`, `.editorconfig`); pin output instead, e.g. `JsonSerializerOptions.NewLine = "\n"` in `RecordTests.SerializeRecord`.
- Use forward-compatible paths (`Path.Combine`), and remember Linux file systems are case-sensitive.
- A test that genuinely cannot run on one platform should skip itself at run time with a reason, not be removed or tagged away: `Assert.SkipUnless(OperatingSystem.IsWindows(), "uses a Windows-only API");` (or `Assert.SkipWhen(...)`) as the first line of the test. The skip then shows in the run summary with its reason on the other platform.

---

## Stack

| Concern | Package / Version |
|---|---|
| Test framework | `xunit.v3` 4.0.3 (runs on Microsoft.Testing.Platform) |
| Parallel execution | Native `[assembly: Parallelization(...)]` in `AssemblyInfo.cs` (replaces `Meziantou.Xunit.v3.ParallelTestFramework`, which does not support xunit 4) |
| VS / Rider runner | `xunit.runner.visualstudio` 4.0.1 |
| Test SDK | `Microsoft.NET.Test.Sdk` 18.6.0 |
| Coverage | `Microsoft.Testing.Extensions.CodeCoverage` 18.12.0 (see Code coverage below) |
| Code under test | `src/CsArena.Core` class library, referenced by the test project |
| Configuration | `Microsoft.Extensions.Configuration.*` 11.0.0 |
| Target framework | `net11.0` |
| Language | C# 15 (`LangVersion=latest`) |
| Build settings | Root `Directory.Build.props`: `TreatWarningsAsErrors`, `Nullable`, `LangVersion` for all projects; the test project adds `NoWarn=NETSDK1233` in its own `Directory.Build.props` |

---

## Project layout

The code under test lives in its own class library, `CsArena.Core`, so the coverage tools instrument it (they skip the test assembly itself). `CsArena.Tests` references it and holds only tests and test infrastructure.

```
src/CsArena.Core/                   # Class library (namespace CsArena.Core.*)
├── LeetCode/                       # Namespace CsArena.Core.LeetCode
│   ├── Arrays.cs, BinarySearch.cs, Backtracking.cs, Bits.cs, Combinatorics.cs, Heap.cs,
│   │   Intervals.cs, Lists.cs, Maths.cs, Sort.cs, Stacks.cs, Strings.cs, Trees.cs, Trie.cs
│   ├── ListNode.cs                 # Linked-list node (used by Lists)
│   ├── StackWithMin.cs             # Helper (used by Stacks)
│   └── TreeNode.cs                 # Binary-tree node (used by Trees)
├── Collections/                    # Namespace CsArena.Core.Collections
│   ├── SkipListCollection.cs
│   ├── SkipListDictionary.cs
│   ├── SkipListSetNode.cs          # internal
│   └── ThrowHelper.cs              # internal
├── Extensions/                     # Namespace CsArena.Core.Extensions
│   ├── IntExtensions.cs
│   ├── LinqExtensions.cs           # MyCount, Filter, SelectWithIndex, Random(seed)
│   └── StringExtensions.cs         # Concat, ReplaceAt, ParseEnum<T>
└── Models/                         # Namespace CsArena.Core.Models
    ├── Book.cs
    ├── Car.cs                      # Two Deconstruct overloads
    ├── Country.cs
    └── Player.cs                   # Indexer via PlayerList wrapper

test/CsArena.Tests/                 # Tests (namespace CsArena.Tests, CsArena.Tests.LeetCode)
│
├── AssemblyInfo.cs                 # Assembly-wide [Parallelization] setting
├── *Tests.cs                       # Feature tests (one file per topic; CSharp14Tests, CSharp15Tests for language features)
├── CollectionUtil.cs               # Static test-data factory (countries, dicts)
├── HashHelpers.cs                  # Hash / equality helpers used by CollectionTests
│
├── config/
│   ├── appsettings.json            # Copied to output; base config values
│   ├── appsettings.local.json      # Copied to output; local overrides
│   ├── Options.cs                  # Strongly-typed config binding target
│   └── TestConfigHelper.cs         # IConfigurationRoot builder; PrintConfig helpers
│
└── LeetCode/
    └── *Tests.cs                   # ArraysTests, BinarySearchTests, ... one per solution class
```

`CsArena.Core` exposes its internals to `CsArena.Tests` (`InternalsVisibleTo`).

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
| `Environment.SetEnvironmentVariable` | **No** | Process-wide: every parallel test that reads the environment sees it. Put such tests in a non-parallel collection (`ConfigurationTests` uses `EnvironmentCollection`) and restore the old values in `finally` |
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

All test classes live in `CsArena.Tests` (root topics) or `CsArena.Tests.LeetCode` (algorithm tests). The code they exercise lives in `CsArena.Core.*` (`.LeetCode`, `.Collections`, `.Extensions`, `.Models`). `Xunit` is a **global implicit using** via:

```xml
<Using Include="Xunit" />   <!-- CsArena.Tests.csproj -->
```

`[Fact]`, `[Theory]`, `Assert`, `ITestOutputHelper`, and `TestContext` are available everywhere. **Never add `using Xunit;` explicitly** — it compiles but is redundant. `using Xunit.Abstractions;` does not exist in xUnit v3; `ITestOutputHelper` moved to the `Xunit` namespace.

Add explicit `using` only for non-Xunit namespaces your file needs:

```csharp
using System.Collections.Concurrent;
using CsArena.Core.Extensions;
using CsArena.Core.Models;
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

### `[NetworkFact]` — tests that reach the network

Skipped unless the environment variable `CSARENA_NETWORK_TESTS=1` is set, and tagged `category=network`. The three tests in `HttpTests` use it instead of a hard-coded `Skip`, so they can be switched on without editing code:

```bash
CSARENA_NETWORK_TESTS=1 dotnet test --project CsArena/test/CsArena.Tests --filter-trait "category=network"   # bash
$env:CSARENA_NETWORK_TESTS = "1"; dotnet test --project CsArena/test/CsArena.Tests --filter-trait "category=network"   # PowerShell
```

`NetworkFactAttribute` sets `Skip`, `SkipUnless` and `SkipType` (xUnit's conditional-skip mechanism); a test of your own can do the same with `[Fact(Skip = "reason", SkipUnless = nameof(Prop), SkipType = typeof(Env))]`.

### `[Fact(Skip = "reason")]` — intentional skip

Only for genuinely non-deterministic scheduler-dependent behaviour, after all determinism fixes have been exhausted. Tests that need an external resource should use a conditional skip (`[NetworkFact]`) instead of an unconditional one.

---

## Test categories

`[Category(TestCategories.X)]` (in `TestCategories.cs`) adds a `category` trait to a test or a whole class. Select with `--filter-trait "category=<name>"`, exclude with `--filter-not-trait "category=<name>"`.

| Category | Applied to | Tests |
|---|---|---|
| `network` | `[NetworkFact]` tests in `HttpTests` | 3 |
| `gc` | `GcTests` (runs alone, non-parallel collection) | 3 |
| `concurrency` | `AsyncTests` (all carry a per-test timeout) | 31 |
| `slow` | reserved for tests of about 300 ms or more; none today (the six that used to qualify were made fast, see "Keeping tests fast") | 0 |
| `leetcode` | every `LeetCode/*Tests` class | 144 |
| `language` | C# language features: `AnonymousTypeTests`, `AttributeTests`, `CSharp14Tests`, `CSharp15Tests`, `DelegateTests`, `ExceptionTests`, `GenericTests`, `IteratorBlockTests`, `LanguageTests`, `NullTests`, `PatternMatchingTests`, `RecordTests`, `TupleTests`, `TypeTests` | 125 |
| `collections` | `CollectionTests`, `SkipListCollectionTests`, `SkipListDictionaryTests` | 81 |
| `linq` | `LinqTests` | 22 |
| `bcl` | base class library APIs: `ConfigurationTests`, `MathTests`, `MemoryTests`, `ReflectionTests`, `TestCategoriesTests` | 34 |

Every test has at least one category, and `TestCategoriesTests` fails with the list of offenders when a new test or class has none (it also checks that only the categories in `TestCategories` are used). Counts include theory rows. A test can have several categories, but none has more than one today, so the counts add up to the 443 tests. Check durations with `--show-slowest-tests`; the slowest tests are about 150 ms, well under the 300 ms that would earn the `slow` tag.

### Categories in Visual Studio Test Explorer

The categories reach Test Explorer (checked in Visual Studio 2026), but the **Traits column cells stay blank** for these tests, so use one of the other views, which all read the same trait data:

- **Header filter:** the funnel icon in the Traits column header lists `category [gc]`, `category [leetcode]`, ... and filters the list when ticked.
- **Group By:** the toolbar's Group By, then **Traits**, gives one group per category plus a "No Traits" group for tests without one.
- **Search box:** `Trait:gc` or `Trait:leetcode` filters by a trait value.

The blank cell is a display quirk on the Visual Studio side, not a missing trait: the test host reports `category=...` for every tagged test, both through the new test platform and through the VSTest adapter, and the command-line `--filter-trait "category=..."` works. If a category never appears in the filter popup, rebuild and refresh Test Explorer (delete `.vs` if the old test list is cached).

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

`TestConfigHelper.PrintConfig(output)` and `PrintConfigProviders(output)` use this pattern; they also return the lines they print, so a test can assert on them (`ConfigurationTests.PrintConfigTest` and `PrintConfigProvidersTest` do) instead of only printing.

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

### Models (`CsArena.Core/Models`)

`Country`, `Player` / `PlayerList`, `Book`, `Car` / `CarStatistics` — plain data objects used as realistic LINQ / reflection / generics subjects. Prefer them over ad-hoc anonymous types when you need a named, reusable shape.

---

## LeetCode sub-project conventions

Each topic has two files, in two projects:

- **`CsArena.Core/LeetCode/Topic.cs`** — static class, static methods, block-comment docstring with problem statement, approach, and complexity.
- **`CsArena.Tests/LeetCode/TopicTests.cs`** — one `[Fact]` or `[Theory]` per problem, named `<ProblemName>Test`. `using static` needs the fully qualified type name (`using static CsArena.Core.LeetCode.Sort;`), because it ignores sibling `using` directives; add `using CsArena.Core.LeetCode;` as well when the test uses types such as `ListNode` or `TreeNode`.

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

Add the implementation to `CsArena.Core/LeetCode/Topic.cs` and the test to `CsArena.Tests/LeetCode/TopicTests.cs`. Use `[Theory]` + `[InlineData]` for parameterized cases.

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
| Test asserts on text containing a line break (JSON, `Environment.NewLine`, raw string literals) | Passes on Windows, fails on Linux or after a line-ending change | Pin the line break (`JsonSerializerOptions.NewLine`) or compare normalized text |
| Named mutex / `dotnet` first run fails in WSL with "does not have the expected owner or permissions" | Tests never start | Set `XDG_RUNTIME_DIR` to a private directory you own (see Platforms and CI) |
| `Task.Delay(n).Wait()` in a sync event handler | Thread-pool pressure delays handler past a racing async continuation (was in `WaitOnAsyncVoid` — fixed) | `Thread.Sleep(n)` or remove |
| `Thread.Sleep(n)` to guarantee task completion before `await` | Timer-resolution race under load (was in `ContinueAfterAwaitInTheSameThread` — fixed) | `while (!task.IsCompleted) Thread.Yield()` |
| Fixed `await Task.Delay(n)` to wait for background tasks | Race when system is slow (was `await Task.Delay(500)` in `AttachedToParentTasks` — fixed) | `await Task.WhenAll(task1!, task2!)` |
| A test that only prints (calls a helper, writes to `ITestOutputHelper`) and asserts nothing | Cannot fail, so it hides breakage; the two `Print...` tests in `ConfigurationTests` were like this and were also unaffected by a leaked environment variable that broke them once they asserted | Make the helper return what it prints and assert on it |
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
