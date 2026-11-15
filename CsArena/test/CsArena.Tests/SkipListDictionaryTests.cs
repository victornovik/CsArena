using CsArena.Core.Collections;

namespace CsArena.Tests;

// SkipListDictionary<TKey, TValue> keeps its pairs sorted by key (a SkipListCollection of pairs compared by key only).
// Add, the indexer and the key-value-pair Contains/Remove follow Dictionary<TKey, TValue>; the tests below the
// "not like Dictionary" marker describe the members that still differ (unimplemented members, unpopulated Keys/Values).
public class SkipListDictionaryTests
{
    private static SkipListDictionary<string, int> Create(params (string Key, int Value)[] pairs)
    {
        var dictionary = new SkipListDictionary<string, int>();
        foreach (var (key, value) in pairs) dictionary.Add(key, value);
        return dictionary;
    }

    // Calls the dictionary's own methods: Assert.Contains would enumerate instead of exercising them.
    private static bool HasKey(IDictionary<string, int> dictionary, string key) => dictionary.ContainsKey(key);

    private static bool HasPair(ICollection<KeyValuePair<string, int>> dictionary, KeyValuePair<string, int> pair) =>
        dictionary.Contains(pair);

    [Fact]
    public void EmptyDictionaryTest()
    {
        var dictionary = Create();

        Assert.Empty(dictionary);
        Assert.False(dictionary.IsReadOnly);
        Assert.False(HasKey(dictionary, "a"));
        Assert.False(dictionary.Remove("a"));
    }

    [Fact]
    public void AddAndIndexerGetTest()
    {
        var dictionary = Create(("b", 2), ("a", 1), ("c", 3));

        Assert.Equal(1, dictionary["a"]);
        Assert.Equal(2, dictionary["b"]);
        Assert.Equal(3, dictionary["c"]);
        Assert.Equal(3, dictionary.Count);
    }

    [Fact]
    public void EnumerationIsSortedByKeyTest()
    {
        var dictionary = Create(("pear", 3), ("apple", 1), ("mango", 2), ("banana", 4));

        Assert.Equal(
            [
                new KeyValuePair<string, int>("apple", 1),
                new KeyValuePair<string, int>("banana", 4),
                new KeyValuePair<string, int>("mango", 2),
                new KeyValuePair<string, int>("pear", 3),
            ],
            dictionary);
    }

    [Fact]
    public void ContainsKeyTest()
    {
        var dictionary = Create(("a", 1), ("b", 2));

        Assert.True(HasKey(dictionary, "a"));
        Assert.True(HasKey(dictionary, "b"));
        Assert.False(HasKey(dictionary, "c"));
    }

    [Fact]
    public void MissingKeyThrowsTest()
    {
        var dictionary = Create(("a", 1));

        Assert.Throws<KeyNotFoundException>(() => dictionary["missing"]);
    }

    [Fact]
    public void NullKeyThrowsTest()
    {
        var dictionary = Create(("a", 1));

        Assert.Throws<ArgumentNullException>(() => dictionary[null!]);
        Assert.Throws<ArgumentNullException>(() => dictionary[null!] = 5);
        Assert.Equal("key", Assert.Throws<ArgumentNullException>(() => dictionary.Add(null!, 5)).ParamName);
        Assert.Throws<ArgumentNullException>(() =>
            ((ICollection<KeyValuePair<string, int>>)dictionary).Add(new KeyValuePair<string, int>(null!, 5)));
        Assert.Single(dictionary);
    }

    [Fact]
    public void IndexerSetAddsNewKeyTest()
    {
        var dictionary = Create();

        dictionary["x"] = 10;
        dictionary["a"] = 20;

        Assert.Equal(10, dictionary["x"]);
        Assert.Equal(20, dictionary["a"]);
        Assert.Equal(["a", "x"], dictionary.Select(pair => pair.Key));
    }

    // Like Dictionary<TKey, TValue>: adding an existing key throws and leaves the stored value alone.
    [Fact]
    public void AddingAnExistingKeyThrowsTest()
    {
        var dictionary = Create(("a", 1));

        var exception = Assert.Throws<ArgumentException>(() => dictionary.Add("a", 99));

        Assert.Equal("key", exception.ParamName);
        Assert.Contains("same key", exception.Message);
        Assert.Equal(1, dictionary["a"]);
        Assert.Single(dictionary);
    }

    // Like Dictionary<TKey, TValue>: the indexer setter replaces the value of an existing key.
    [Fact]
    public void IndexerSetOverwritesAnExistingKeyTest()
    {
        var dictionary = Create(("a", 1), ("b", 2));

        dictionary["a"] = 99;

        Assert.Equal(99, dictionary["a"]);
        Assert.Equal(2, dictionary["b"]);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal(
            [new KeyValuePair<string, int>("a", 99), new KeyValuePair<string, int>("b", 2)],
            dictionary);
    }

    // Overwriting a value is done in place, so a running enumeration is not invalidated
    // (Dictionary<TKey, TValue> behaves the same way since .NET Core 3.0).
    [Fact]
    public void IndexerSetDuringEnumerationDoesNotInvalidateItTest()
    {
        var dictionary = Create(("a", 1), ("b", 2), ("c", 3));

        var seen = new List<string>();
        foreach (var (key, value) in dictionary)
        {
            dictionary[key] = value * 10;
            seen.Add(key);
        }

        Assert.Equal(["a", "b", "c"], seen);
        Assert.Equal(30, dictionary["c"]);
    }

    [Fact]
    public void RemoveByKeyTest()
    {
        var dictionary = Create(("a", 1), ("b", 2), ("c", 3));

        Assert.True(dictionary.Remove("b"));

        Assert.False(HasKey(dictionary, "b"));
        Assert.Equal(["a", "c"], dictionary.Select(pair => pair.Key));
        Assert.Equal(2, dictionary.Count);
        Assert.False(dictionary.Remove("b"));
        Assert.False(dictionary.Remove("zzz"));
    }

    [Fact]
    public void RemovedKeyCanBeAddedAgainWithANewValueTest()
    {
        var dictionary = Create(("a", 1));

        Assert.True(dictionary.Remove("a"));
        dictionary.Add("a", 2);

        Assert.Equal(2, dictionary["a"]);
    }

    [Fact]
    public void KeyValuePairAddTest()
    {
        ICollection<KeyValuePair<string, int>> dictionary = Create();

        dictionary.Add(new KeyValuePair<string, int>("k", 7));

        Assert.Equal(7, ((IDictionary<string, int>)dictionary)["k"]);
        Assert.Single(dictionary);
        Assert.Throws<ArgumentException>(() => dictionary.Add(new KeyValuePair<string, int>("k", 8)));
        Assert.Equal(7, ((IDictionary<string, int>)dictionary)["k"]);
    }

    // Like Dictionary<TKey, TValue>: a pair is "contained" only when the key exists AND the value equals the stored one.
    [Fact]
    public void KeyValuePairContainsMatchesKeyAndValueTest()
    {
        ICollection<KeyValuePair<string, int>> dictionary = Create(("a", 1), ("b", 2));

        Assert.True(HasPair(dictionary, new KeyValuePair<string, int>("a", 1)));
        Assert.True(HasPair(dictionary, new KeyValuePair<string, int>("b", 2)));
        Assert.False(HasPair(dictionary, new KeyValuePair<string, int>("a", 2)));      // key found, other value
        Assert.False(HasPair(dictionary, new KeyValuePair<string, int>("a", 12345)));
        Assert.False(HasPair(dictionary, new KeyValuePair<string, int>("z", 1)));      // key missing
    }

    [Fact]
    public void KeyValuePairRemoveMatchesKeyAndValueTest()
    {
        ICollection<KeyValuePair<string, int>> dictionary = Create(("a", 1), ("b", 2), ("c", 3));

        // Wrong value: nothing is removed
        Assert.False(dictionary.Remove(new KeyValuePair<string, int>("a", 12345)));
        Assert.Equal(3, dictionary.Count);

        // Missing key: nothing is removed
        Assert.False(dictionary.Remove(new KeyValuePair<string, int>("z", 1)));

        // Matching key and value: removed
        Assert.True(dictionary.Remove(new KeyValuePair<string, int>("b", 2)));
        Assert.Equal(["a", "c"], ((IDictionary<string, int>)dictionary).Select(pair => pair.Key));
        Assert.False(dictionary.Remove(new KeyValuePair<string, int>("b", 2)));
    }

    // Values are compared with EqualityComparer<TValue>.Default, so reference types compare by their Equals.
    [Fact]
    public void KeyValuePairValueIsComparedWithDefaultEqualityTest()
    {
        var dictionary = new SkipListDictionary<int, string>();
        dictionary.Add(1, new string("one".ToCharArray()));   // a distinct string instance
        ICollection<KeyValuePair<int, string>> pairs = dictionary;

        Assert.True(pairs.Contains(new KeyValuePair<int, string>(1, "one")));
        Assert.False(pairs.Contains(new KeyValuePair<int, string>(1, "ONE")));
        Assert.True(pairs.Remove(new KeyValuePair<int, string>(1, "one")));
        Assert.Empty(dictionary);
    }

    [Fact]
    public void KeyValuePairContainsAndRemoveWithNullKeyThrowTest()
    {
        ICollection<KeyValuePair<string, int>> dictionary = Create(("a", 1));
        var nullKeyPair = new KeyValuePair<string, int>(null!, 1);

        Assert.Equal("key", Assert.Throws<ArgumentNullException>(() => dictionary.Contains(nullKeyPair)).ParamName);
        Assert.Equal("key", Assert.Throws<ArgumentNullException>(() => dictionary.Remove(nullKeyPair)).ParamName);
        Assert.Single(dictionary);
    }

    [Fact]
    public void KeyValuePairMembersUseTheComparerForKeysTest()
    {
        ICollection<KeyValuePair<string, int>> dictionary = new SkipListDictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "Key", 1 },
        };

        Assert.True(dictionary.Contains(new KeyValuePair<string, int>("KEY", 1)));
        Assert.False(dictionary.Contains(new KeyValuePair<string, int>("KEY", 2)));
        Assert.True(dictionary.Remove(new KeyValuePair<string, int>("kEy", 1)));
        Assert.Empty(dictionary);
    }

    [Fact]
    public void CustomComparerTest()
    {
        var dictionary = new SkipListDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        dictionary.Add("Key", 1);
        dictionary.Add("other", 3);

        // The comparer decides what the same key is
        Assert.Throws<ArgumentException>(() => dictionary.Add("KEY", 2));
        Assert.Equal(1, dictionary["kEy"]);
        Assert.True(HasKey(dictionary, "OTHER"));

        // Overwriting keeps the originally stored key spelling, like Dictionary<,> does
        dictionary["KEY"] = 5;
        Assert.Equal(["Key", "other"], dictionary.Select(pair => pair.Key));
        Assert.Equal(5, dictionary["key"]);

        Assert.True(dictionary.Remove("KEY"));
        Assert.Equal(["other"], dictionary.Select(pair => pair.Key));
    }

    [Fact]
    public void DescendingComparerOrdersEnumerationTest()
    {
        var dictionary = new SkipListDictionary<int, string>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
        foreach (var key in new[] { 1, 3, 2 }) dictionary.Add(key, $"v{key}");

        Assert.Equal([3, 2, 1], dictionary.Select(pair => pair.Key));
    }

    [Fact]
    public void NullComparerFallsBackToDefaultTest()
    {
        var dictionary = new SkipListDictionary<string, int>(null!);
        dictionary.Add("b", 2);
        dictionary.Add("a", 1);

        Assert.Equal(["a", "b"], dictionary.Select(pair => pair.Key));
    }

    [Fact]
    public void NonGenericEnumeratorTest()
    {
        System.Collections.IEnumerable dictionary = Create(("b", 2), ("a", 1));

        Assert.Equal(
            [new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2)],
            dictionary.Cast<KeyValuePair<string, int>>());
    }

    // ---- Not like Dictionary<TKey, TValue> ----

    [Fact]
    public void NotImplementedMembersTest()
    {
        var dictionary = Create(("a", 1));

        Assert.Throws<NotImplementedException>(() => dictionary.Clear());
        Assert.Throws<NotImplementedException>(() => dictionary.CopyTo(new KeyValuePair<string, int>[1], 0));
        Assert.Throws<NotImplementedException>(() => dictionary.TryGetValue("a", out _));
    }

    // Keys and Values are declared but never populated.
    [Fact]
    public void KeysAndValuesAreNotPopulatedTest()
    {
        var dictionary = Create(("a", 1));

        Assert.Null(dictionary.Keys);
        Assert.Null(dictionary.Values);
    }

    // Randomised operations are replayed against SortedDictionary<,> as a reference model.
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(99)]
    public void MatchesSortedDictionaryUnderRandomOperationsTest(int seed)
    {
        var random = new Random(seed);
        var dictionary = new SkipListDictionary<int, int>();
        var model = new SortedDictionary<int, int>();

        for (var i = 0; i < 1_500; i++)
        {
            var key = random.Next(0, 200);
            switch (random.Next(0, 4))
            {
                case 0:
                    Assert.Equal(model.Remove(key), dictionary.Remove(key));
                    break;
                case 1:
                    model[key] = i;
                    dictionary[key] = i;
                    break;
                default:
                    if (model.ContainsKey(key))
                    {
                        Assert.Throws<ArgumentException>(() => dictionary.Add(key, i));
                    }
                    else
                    {
                        model.Add(key, i);
                        dictionary.Add(key, i);
                    }
                    break;
            }

            Assert.Equal(model.Count, dictionary.Count);
        }

        Assert.Equal(model, dictionary);
        foreach (var (key, value) in model)
        {
            Assert.Equal(value, dictionary[key]);
        }
    }
}
