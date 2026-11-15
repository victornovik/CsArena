using System.Runtime.CompilerServices;
using Xunit.v3;

namespace CsArena.Tests;

/// <summary>
/// Category names used with <see cref="CategoryAttribute"/>. Select them with
/// <c>dotnet test --filter-trait "category=gc"</c> or exclude them with <c>--filter-not-trait "category=concurrency"</c>.
/// </summary>
public static class TestCategories
{
    public const string TraitName = "category";

    /// <summary>Reaches a real network endpoint; opt in with <see cref="TestEnvironment.NetworkVariable"/>=1.</summary>
    public const string Network = "network";

    /// <summary>Asserts on garbage-collector state; runs alone in the non-parallel "gc" collection.</summary>
    public const string Gc = "gc";

    /// <summary>Threads, tasks, locks and OS primitives; carries a per-test timeout.</summary>
    public const string Concurrency = "concurrency";

    /// <summary>Takes roughly 300 ms or more on a developer machine. Reserved: no test needs it today.</summary>
    public const string Slow = "slow";

    /// <summary>Algorithm tests for the solutions in CsArena.Core.LeetCode.</summary>
    public const string LeetCode = "leetcode";

    /// <summary>C# language features: types, generics, delegates, pattern matching, records, tuples, nullability, ...</summary>
    public const string Language = "language";

    /// <summary>Collection types: the BCL collections and the skip-list collections in CsArena.Core.</summary>
    public const string Collections = "collections";

    /// <summary>LINQ operators, query syntax and custom operators.</summary>
    public const string Linq = "linq";

    /// <summary>Base class library APIs outside the groups above: configuration, math, memory and spans, reflection.</summary>
    public const string Bcl = "bcl";
}

/// <summary>Tags a test class or method with a <c>category</c> trait, e.g. <c>[Category(TestCategories.Gc)]</c>.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CategoryAttribute(string name) : Attribute, ITraitAttribute
{
    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [new KeyValuePair<string, string>(TestCategories.TraitName, name)];
}

public static class TestEnvironment
{
    public const string NetworkVariable = "CSARENA_NETWORK_TESTS";

    /// <summary>True when the environment variable <see cref="NetworkVariable"/> is set to 1.</summary>
    public static bool NetworkTestsEnabled => Environment.GetEnvironmentVariable(NetworkVariable) == "1";
}

/// <summary>
/// A test that needs the network: skipped unless <c>CSARENA_NETWORK_TESTS=1</c> is set, and tagged
/// <c>category=network</c> so it can also be filtered by trait.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class NetworkFactAttribute : FactAttribute, ITraitAttribute
{
    public NetworkFactAttribute([CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        Skip = $"Network test: set {TestEnvironment.NetworkVariable}=1 to run it";
        SkipUnless = nameof(TestEnvironment.NetworkTestsEnabled);
        SkipType = typeof(TestEnvironment);
    }

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() =>
        [new KeyValuePair<string, string>(TestCategories.TraitName, TestCategories.Network)];
}
