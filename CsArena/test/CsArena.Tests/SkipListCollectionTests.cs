using System.Collections;
using CsArena.Core.Collections;

namespace CsArena.Tests;

// SkipListCollection<T> is a sorted set built on a skip list: no duplicates, ordered by the comparer.
// The random tower heights come from a seeded Random, so every run builds the same structure.
[Category(TestCategories.Collections)]
public class SkipListCollectionTests
{
    private static SkipListCollection<int> CreateInts(params int[] items)
    {
        var list = new SkipListCollection<int>(Comparer<int>.Default);
        foreach (var item in items) list.Add(item);
        return list;
    }

    // Calls the collection's own Contains: Assert.Contains would enumerate instead of exercising it.
    private static bool Has<T>(ICollection<T> collection, T item) => collection.Contains(item);

    [Fact]
    public void EmptyCollectionTest()
    {
        var list = CreateInts();

        Assert.Empty(list);
        Assert.False(list.IsReadOnly);
        Assert.False(Has(list, 1));
        Assert.False(list.Remove(1));
    }

    [Fact]
    public void AddKeepsItemsSortedTest()
    {
        var list = CreateInts(5, 1, 4, 2, 3, -7, 100);

        Assert.Equal([-7, 1, 2, 3, 4, 5, 100], list);
        Assert.Equal(7, list.Count);
    }

    [Fact]
    public void AddIgnoresDuplicatesTest()
    {
        var list = CreateInts(3, 1, 3, 2, 1, 3);

        Assert.Equal([1, 2, 3], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void TryAddReportsWhetherTheItemWasInsertedTest()
    {
        var list = CreateInts(1, 3);

        Assert.True(list.TryAdd(2));
        Assert.False(list.TryAdd(2));
        Assert.False(list.TryAdd(1));
        Assert.Equal([1, 2, 3], list);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void ContainsTest()
    {
        var list = CreateInts(10, 20, 30);

        Assert.True(Has(list, 10));
        Assert.True(Has(list, 30));
        Assert.False(Has(list, 15));
        Assert.False(Has(list, 0));
        Assert.False(Has(list, 31));
    }

    [Fact]
    public void RemoveTest()
    {
        var list = CreateInts(1, 2, 3, 4, 5);

        Assert.True(list.Remove(3));
        Assert.Equal([1, 2, 4, 5], list);
        Assert.Equal(4, list.Count);
        Assert.False(Has(list, 3));

        // Removing again, or removing something that was never there, changes nothing
        Assert.False(list.Remove(3));
        Assert.False(list.Remove(99));
        Assert.Equal(4, list.Count);
    }

    [Fact]
    public void RemoveFirstAndLastTest()
    {
        var list = CreateInts(1, 2, 3);

        Assert.True(list.Remove(1));
        Assert.True(list.Remove(3));

        Assert.Equal([2], list);
    }

    [Fact]
    public void RemoveEverythingThenReuseTest()
    {
        var list = CreateInts(1, 2, 3);
        foreach (var item in new[] { 2, 3, 1 }) Assert.True(list.Remove(item));

        Assert.Empty(list);

        list.Add(42);
        Assert.Equal([42], list);
    }

    [Fact]
    public void CustomComparerTest()
    {
        var descending = new SkipListCollection<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        foreach (var item in new[] { 2, 5, 1, 4, 3 }) descending.Add(item);

        Assert.Equal([5, 4, 3, 2, 1], descending);
        Assert.True(Has(descending, 4));
        Assert.True(descending.Remove(5));
        Assert.Equal([4, 3, 2, 1], descending);
    }

    [Fact]
    public void ComparerDecidesEqualityTest()
    {
        var list = new SkipListCollection<string>(Comparer<string>.Create(StringComparer.OrdinalIgnoreCase.Compare));
        list.Add("Apple");
        list.Add("APPLE");
        list.Add("banana");

        Assert.Equal(["Apple", "banana"], list);
        Assert.True(Has(list, "aPpLe"));
        Assert.True(list.Remove("BANANA"));
        Assert.Equal(["Apple"], list);
    }

    [Fact]
    public void FindNodeTest()
    {
        var list = CreateInts(1, 2, 3);

        var node = list.FindNode(2);
        Assert.NotNull(node);
        Assert.Equal(2, node.Key);
        Assert.Null(list.FindNode(5));
    }

    [Fact]
    public void NonGenericEnumeratorTest()
    {
        IEnumerable list = CreateInts(2, 1);

        Assert.Equal([1, 2], list.Cast<int>());
    }

    // The version check runs before each element is returned, so a change is noticed on the next MoveNext
    // as long as another element is still to come.
    [Fact]
    public void AddWhileEnumeratingThrowsTest()
    {
        var list = CreateInts(1, 2, 3);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            foreach (var item in list)
            {
                if (item == 1) list.Add(10);
            }
        });
        Assert.Contains("modified", exception.Message);
    }

    [Fact]
    public void RemoveWhileEnumeratingThrowsTest()
    {
        var list = CreateInts(1, 2, 3);

        Assert.Throws<InvalidOperationException>(() =>
        {
            foreach (var item in list)
            {
                if (item == 1) list.Remove(2);
            }
        });
    }

    [Fact]
    public void ModificationWithNoFollowingElementIsNotDetectedTest()
    {
        var list = CreateInts(1, 2, 3);

        // Adding 0 while on the last element puts the new node before it: nothing follows 3,
        // so the enumerator ends without re-checking the version.
        foreach (var item in list)
        {
            if (item == 3) list.Add(0);
        }
        Assert.Equal([0, 1, 2, 3], list);

        // Adding a larger value creates a following node, so the next MoveNext notices the change.
        Assert.Throws<InvalidOperationException>(() =>
        {
            foreach (var item in list)
            {
                if (item == 3) list.Add(10);
            }
        });
    }

    [Fact]
    public void FailedAddOrRemoveDoesNotInvalidateEnumeratorTest()
    {
        var list = CreateInts(1, 2, 3);

        var seen = new List<int>();
        foreach (var item in list)
        {
            list.Add(item);    // duplicate, ignored
            list.Remove(-1);   // missing, ignored
            seen.Add(item);
        }

        Assert.Equal([1, 2, 3], seen);
    }

    [Fact]
    public void ClearAndCopyToAreNotImplementedTest()
    {
        var list = CreateInts(1, 2, 3);

        Assert.Throws<NotImplementedException>(() => list.Clear());
        Assert.Throws<NotImplementedException>(() => list.CopyTo(new int[3], 0));
    }

    [Fact]
    public void LargeCollectionStaysSortedTest()
    {
        var shuffled = Enumerable.Range(0, 5_000).OrderBy(n => (n * 7919) % 5_003).ToArray();
        var list = CreateInts(shuffled);

        Assert.Equal(5_000, list.Count);
        Assert.Equal(Enumerable.Range(0, 5_000), list);
        Assert.True(Has(list, 0));
        Assert.True(Has(list, 4_999));
        Assert.False(Has(list, 5_000));
    }

    // Randomised operations are replayed against SortedSet<T> as a reference model.
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(1234)]
    public void MatchesSortedSetUnderRandomOperationsTest(int seed)
    {
        var random = new Random(seed);
        var list = new SkipListCollection<int>(Comparer<int>.Default, seed);
        var model = new SortedSet<int>();

        for (var i = 0; i < 2_000; i++)
        {
            var value = random.Next(0, 300);
            if (random.Next(0, 3) == 0)
            {
                Assert.Equal(model.Remove(value), list.Remove(value));
            }
            else
            {
                model.Add(value);
                list.Add(value);
            }

            Assert.Equal(model.Count, list.Count);
        }

        Assert.Equal(model, list);
        for (var value = 0; value < 300; value++)
        {
            Assert.Equal(model.Contains(value), list.Contains(value));
        }
    }

    [Fact]
    public void SameSeedBuildsTheSameStructureTest()
    {
        static SkipListCollection<int> Build(int seed)
        {
            var list = new SkipListCollection<int>(Comparer<int>.Default, seed);
            for (var i = 0; i < 200; i++) list.Add(i);
            return list;
        }

        Assert.Equal(Build(7).DebugString, Build(7).DebugString);
        Assert.Equal(Build(7).LevelStats, Build(7).LevelStats);
    }

    // LevelStats prints "<tower height>: <node count>" lines. The head sentinel is counted too,
    // and it always has the maximum height of 21 (levels 0..20).
    [Fact]
    public void LevelStatsCountsEveryNodeIncludingTheHeadTest()
    {
        var list = new SkipListCollection<int>(Comparer<int>.Default, seed: 5);
        for (var i = 0; i < 100; i++) list.Add(i);

        var lines = list.LevelStats.Split(Environment.NewLine);
        var counts = lines.Select(line => line.Split(": ")).ToDictionary(p => int.Parse(p[0]), p => int.Parse(p[1]));

        Assert.Equal(1, counts[21]);
        Assert.Equal(list.Count + 1, counts.Values.Sum());
        // Heights are listed in ascending order
        Assert.Equal(counts.Keys.Order(), counts.Keys);
        // About half of the nodes stop at the first level
        Assert.True(counts[1] > 25 && counts[1] < 75, $"unexpected number of height-1 nodes: {counts[1]}");
    }

    [Fact]
    public void DebugStringListsKeysInOrderOnTheBottomLevelTest()
    {
        var list = CreateInts(3, 1, 2);

        // The first line is the bottom level; it starts at the head sentinel, whose key is default(int).
        var bottomLevel = list.DebugString.Split(Environment.NewLine)[0];

        Assert.Equal("0   1   2   3   ", bottomLevel);
    }
}
