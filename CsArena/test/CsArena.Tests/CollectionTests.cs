/*
+---------------------+---------+----------+----------+----------+------------+---------------+------------+
| Collection          | Indexed | Keyed    | Add \    |  Remove  | Underlying | Lookup        | Contiguous |
|                     | lookup  | lookup   | Insert   |          | structure  | strategy      | storage    |
+---------------------+---------+----------+----------+----------+------------+---------------+------------+
| []                  | O(1)    |   n\a    |          |          | array      | Addr+idx*size | Yes        |
| List                | O(1)    |   n\a    | O(1)\O(n)| O(n)     | array      |               | Yes        |
| SortedList          | O(1)    | O(log n) | O(n)     | O(n)     | 2 arrays   | Binary search | Yes        |
| ObservableCollection| O(1)    |   n\a    | O(1)\O(n)| O(n)     | List<T>    |               | Yes        |
| LinkedList          | O(n)    |   n\a    | O(1)     | O(1)     | doubly linked list         | No         |
| ImmutableList       | O(log n)|   n\a    | O(log n) | O(log n) | BST        | Thread-safe   | No         | 
| ReadOnlyCollection  | O(1)    |   n\a    | O(1)\O(n)| O(n)     | List<T>    |               | Yes        |
| Stack               | n\a     |   n\a    | O(1)     | O(1)     | array      |               | Yes        |
| ImmutableStack      | n\a     |   n\a    | O(1)     | O(1)     | linked list| Thread-safe   | No         |
| Queue               | n\a     |   n\a    | O(1)     | O(1)     | array      |               | Yes        |  
| ImmutableQueue      | n\a     |   n\a    |          |          | 3 immutable stacks         | No         |
| PriorityQueue       | n\a     |   n\a    | O(log n) | O(log n) | quad tree in array         | Yes        |
| HashSet             | n\a     | O(1)     | O(1)     | O(1)     | array of lists + remainder | No         |
| ReadonlySet         | n\a     | O(1)     | O(1)     | O(1)     | ISet                       | No         |
| ImmutableHashSet    | n\a     |   n\a    |          |          |            | Thread-safe   | No         |
| SortedSet           | n\a     |   n\a    | O(log n) | O(log n) | BST        | Binary search | No         |
| ImmutableSortedSet  | n\a     |   n\a    |          |          |            | Thread-safe   | No         |
| FrozenSet           | n\a     | O(1)     | n\a      | n\a      | depends on key length, set length etc
| Dictionary          | n\a     | O(1)     | O(1)     | O(1)     | array of lists + remainder | No         |
| SortedDictionary    | O(n)    | O(log n) | O(log n) | O(log n) | BST        | Binary search | No         |
| ReadOnlyDictionary  | n\a     | O(1)     | O(1)     | O(1)     | dictionary |               | No         |
| ImmutableDictionary | n\a     | O(log n) | O(log n) | O(log n) | BST        | Thread-safe   | No         |
| FrozenDictionary    | n\a     | O(1)     | n\a      | n\a      | depends on key length, dictionary length etc
| ConcurrentQueue     | n\a     |   n\a    |          |          | lock-free, linked list of bounded ring buffers, lock-free Compare-And-Swap  
| ConcurrentStack     | n\a     |   n\a    |          |          | lock-free, linked list, lock-free Compare-And-Swap  
| ConcurrentDictionary| n\a     |          |          |          | lock-free for read, fine-grained for write
| ConcurrentBag       | n\a     |   n\a    |          |          | lock-free for read, local queue per thread
+---------------------+---------+----------+----------+----------+------------+---------------+------------+
*/

using CollectionArena.models;
using CsArena.Tests.ext;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;

namespace CsArena.Tests;

public class CollectionTests
{
    [Fact]
    public void ModifyArray()
    {
        string[] days = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"];

        days[1] = "X";
        Assert.Equal("X", days[1]);

        days.SetValue("Tue", 1);
        Assert.Equal("Tue", days[1]);

        Array.Sort(days);
        Assert.Equal(["Fri", "Mon", "Sat", "Sun", "Thu", "Tue", "Wed"], days);

        Array.Resize(ref days, days.Length - 3);
        Assert.Equal("Sun", days[^1]);
    }

    [Fact]
    public void JaggedArray()
    {
        // Jagged array may contain arrays of different dimensions
        string[][] days =
        [
            new string[1],
            new string[2],
            new string[3]
        ];
        days[0][0] = "Mon";
        days[1][1] = "Wed";
        days[2][2] = "Fri";

        // The array contains 3 arrays
        Assert.Equal(3, days.Length);
        Assert.Single(days[0]);
        Assert.Equal(2, days[1].Length);
        Assert.Equal(3, days[2].Length);
    }

    [Fact]
    public void MultidimensionalArray()
    {
        // Multidimensional array has to contain rows of the same dimension
        string[,] days = new string[3, 3];
        days[0, 0] = "Mon";
        days[1, 1] = "Wed";
        days[2, 2] = "Fri";

        Assert.Equal(9, days.Length);
        Assert.Equal(2, days.GetUpperBound(0));
    }

    [Fact]
    public void List()
    {
        var days = new List<string>();
        Assert.Empty(days);
        Assert.Equal(0, days.Capacity);

        days.Add("Mon");
        Assert.Single(days);

        days.Add("Tue");
        days.AddRange(["Wed", "Thu", "Fri"]);
        Assert.Equal("Fri", days[^1]);
    }

    [Fact]
    public void ListCapacity()
    {
        var days = new List<string>();
        Assert.Equal(0, days.Capacity);

        days.Add("Mon");
        Assert.Single(days);
        Assert.Equal(4, days.Capacity);

        // The Capacity is doubled
        Assert.Equal(8, days.EnsureCapacity(5));
        Assert.Equal(16, days.EnsureCapacity(15));
    }

    [Fact]
    public void ListIntersection()
    {
        List<int> a = [1, 2, 3, 4, 5];
        List<int> b = [3, 4, 5, 6, 7, 8];

        Assert.Equal([3, 4, 5], a.Intersect(b));
    }

    [Fact]
    public void ReadonlyList()
    {
        var rw = CollectionUtil.CreateCountryList();

        ReadOnlyCollection<Country> ro = rw.AsReadOnly();

        Assert.Equal(rw, ro);
        for (var i = 0; i < rw.Count; i++)
        {
            Assert.Equal(rw[i], ro[i]);
        }

        // ReadOnlyCollection actually wraps around underlying list and reflects its changes
        rw.Add(new Country("United Kingdom", "UK", "Europe", 44_000_000));
        Assert.Equal(rw.Count, ro.Count);
        Assert.Equal("UK", ro[3].Code);
    }

    [Fact]
    public void ImmutableList()
    {
        var rw = CollectionUtil.CreateCountryList();

        ImmutableList<Country> ro = rw.ToImmutableList();
        Assert.Equal(rw.Count, ro.Count);

        Assert.Equal(rw, ro);
        for (var i = 0; i < rw.Count; i++)
        {
            Assert.Equal(rw[i], ro[i]);
        }

        rw.Add(new Country("United Kingdom", "UK", "Europe", 44_000_000));
        Assert.Equal(rw.Count - 1, ro.Count);
    }

    [Fact]
    public void SortedList()
    {
        var countries = CollectionUtil.CreateSortedCountryList();

        int i = 0;

        // Countries are now sorted by key
        foreach (var (key, _) in countries)
        {
            Assert.True(i++ switch
            {
                0 => key == "FIN",
                1 => key == "NOR",
                2 => key == "RUS",
                _ => false
            });
        }
    }

    [Fact]
    public void LinkedList()
    {
        var days = new LinkedList<string>(["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);
        var wed = days.Find("Wed");

        Assert.NotNull(wed);
        Assert.Equal("Mon", days.First!.Value);
        Assert.Equal("Sun", days.Last!.Value);

        days.AddBefore(wed, "Weekend");
        days.AddAfter(wed, "Weekend");

        Assert.Equal(9, days.Count);
        Assert.NotNull(wed.Next);
        Assert.Equal("Weekend", wed.Next.Value);
        Assert.NotNull(wed.Previous);
        Assert.Equal("Weekend", wed.Previous.Value);

        var last = days.Last;

        Assert.NotNull(last);
        Assert.Null(last.Next);
        Assert.Same(days, last.List);

        days.AddLast("Extra Sun");

        Assert.Equal(10, days.Count);
        Assert.NotNull(last.Next);
        Assert.Equal("Extra Sun", last.Next.Value);

        days.Remove("Weekend");

        Assert.Equal(9, days.Count);
        Assert.Equal("Weekend", wed.Next.Value);
        Assert.Equal("Tue", wed.Previous.Value);

        days.Remove("Weekend");

        Assert.Equal(8, days.Count);
        Assert.Equal("Thu", wed.Next.Value);
    }

    [Fact]
    public void Dictionary()
    {
        var countries = CollectionUtil.CreateCountryDictionary();

        foreach (var (key, value) in countries)
        {
            Assert.True(key is "NOR" or "FIN" or "RUS");
            Assert.True(value.Name is "Norway" or "Finland" or "Russia");
        }

        // Case-insensitive comparer
        Assert.True(countries.ContainsKey("FIN") == countries.ContainsKey("fiN"));
        // Non-existing key
        Assert.False(countries.ContainsKey("mus"));
        // Duplicate key
        Assert.Throws<ArgumentException>(() => countries.Add("NOR", new Country("", "", "", 0)));
    }

    [Fact]
    public void DictionaryInit()
    {
        // Object initializer with indexer
        Dictionary<string, string> countries = new()
        {
            ["ru"] = "Russia",
            ["us"] = "United States",
            ["uk"] = "Great Britain"
        };

        foreach (var (key, _) in countries)
        {
            Assert.True(key is "ru" or "us" or "uk");
        }

        // Case-sensitive comparer is used by default
        Assert.False(countries.ContainsKey("ru") == countries.ContainsKey("RU"));

        // Mixed initialization
        var europe = new List<KeyValuePair<string, string>> { new("ge", "Germany") };
        var countries2 = new Dictionary<string, string>(europe)
        {
            ["ru"] = "Russia",
            ["us"] = "United States",
            ["uk"] = "Great Britain"
        };

        Assert.Equal("Germany", countries2["ge"]);
    }

    [Fact]
    public void DictionaryInitCollectionInitializerVsObjectInitializer()
    {
        // Collection initializer code calls Add() on Dictionary for each added element.
        // Add() throws AE when the same key is being added.
        var ae = Assert.Throws<ArgumentException>(() =>
        {
            var collectionInitializer = new Dictionary<string, int>()
            {
                { "A", 1 },
                { "B", 2 },
                { "B", 3 }
            };
        });
        Assert.Equal("An item with the same key has already been added. Key: B", ae.Message);

        // Object initializer adds each element by calling indexer [].
        // Collection indexers just silently rewrite already existing elements.
        var objectInitializer = new Dictionary<string, int>()
        {
            ["A"] = 1,
            ["B"] = 2,
            ["B"] = 3
        };
        Assert.Equal(3, objectInitializer["B"]);
    }

    [Fact]
    public void DictionaryCapacity()
    {
        Dictionary<string, string> countries = new();

        var nextPrime = HashHelpers.GetPrime(4);
        Assert.Equal(7, nextPrime);
        Assert.Equal(nextPrime, countries.EnsureCapacity(4));

        nextPrime = HashHelpers.GetPrime(40);
        Assert.Equal(47, nextPrime);
        Assert.Equal(nextPrime, countries.EnsureCapacity(40));
    }

    [Fact]
    public void ImmutableDictionary()
    {
        var rw = CollectionUtil.CreateCountryDictionary();
        var im1 = rw.ToImmutableDictionary();

        Assert.Equal(rw, im1);

        foreach (var key in rw.Keys)
        {
            Assert.Equal(im1[key], rw[key]);
        }

        rw.Add("UK", new Country("United Kingdom", "UK", "Europe", 44_000_000));
        // Immutable dictionary does not contain "UK" key
        Assert.False(im1.ContainsKey("UK"));

        // New immutable dictionary is created due to mutating already existing immutable dictionary
        var im2 = im1.Add("UK", new Country("United Kingdom", "UK", "Europe", 44_000_000));
        Assert.False(ReferenceEquals(im1, im2));
        // Previous dictionary stays immutable
        Assert.False(im1.ContainsKey("UK"));
        Assert.True(im2.ContainsKey("UK"));

        var im3 = im2.Remove("FIN");
        Assert.False(ReferenceEquals(im3, im2));
        // Previous dictionary stays immutable
        Assert.True(im2.ContainsKey("FIN"));
        Assert.False(im3.ContainsKey("FIN"));
    }

    [Fact]
    public void ReadonlyDictionary()
    {
        var rw = CollectionUtil.CreateCountryDictionary();
        var @readonly = new ReadOnlyDictionary<string, Country>(rw);
        Assert.Equal(rw.Count, @readonly.Count);

        // ReadonlyDictionary actually wraps around underlying dictionary and reflects its changes
        rw.Add("UK", new Country("United Kingdom", "UK", "Europe", 44_000_000));
        Assert.Equal(rw["UK"], @readonly["UK"]);
    }

    [Fact]
    public void FrozenDictionary()
    {
        /*
         * FrozenDictionary<TKey,TValue> is immutable and is optimized for situations where a dictionary is created infrequently but is used frequently at run time.
         * It has a relatively high cost to create but provides excellent lookup performance.
         * Thus, it is ideal for cases where a dictionary is created once, potentially at the startup of an application
         * and is used throughout the remainder of the application life.
         */
        var rw = CollectionUtil.CreateCountryDictionary();
        // Immutable, read-only dictionary optimized for fast lookup and enumeration
        var frozen = rw.ToFrozenDictionary();
        Assert.Equal(rw.Count, frozen.Count);

        rw.Add("UK", new Country("United Kingdom", "UK", "Europe", 44_000_000));
        Assert.Equal(rw.Count, frozen.Count + 1);
        Assert.False(frozen.ContainsKey("UK"));
    }

    [Fact]
    public void SortedDictionary()
    {
        var countries = CollectionUtil.CreateSortedCountryDictionary();

        int i = 0;

        // Countries are now sorted by key
        foreach (var (key, _) in countries)
        {
            Assert.True(i++ switch
            {
                0 => key == "FIN",
                1 => key == "NOR",
                2 => key == "RUS",
                _ => false
            });
        }
    }

    [Fact]
    public void HashSet()
    {
        int[] numbers = [5, 6, 3, 2, 1, 5, 6, 7, 8, 4, 6, 1, 5, 7, 3, 4, 5, 6, 7];

        var countOfUnique = numbers.Max() - numbers.Min() + 1;

        var list = numbers.Distinct().ToList();
        Assert.Equal(countOfUnique, list.Count);

        var set = new HashSet<int>(numbers);
        Assert.Equal(countOfUnique, set.Count);

        var (set1, set2) = InitSets();

        Assert.True(set1.Overlaps(set2));

        set1.IntersectWith(set2); // [1]
        Assert.Single(set1);
        Assert.Contains(1, set1);
        Assert.True(set1.IsSubsetOf(set2));
        Assert.True(set2.IsSupersetOf(set1));

        (set1, set2) = InitSets();

        set1.UnionWith(set2);   // [1,2,3,4,5,6,7,8,9]
        Assert.Equal(9, set1.Count);
        Assert.True(set2.IsSubsetOf(set1));
        Assert.True(set1.IsSupersetOf(set2));

        (set1, set2) = InitSets();

        set1.SymmetricExceptWith(set2); // [2,3,4,5,6,7,8,9]
        Assert.Equal(8, set1.Count);
        Assert.DoesNotContain(1, set1);

        set1.ExceptWith(set2); // [2,3,4,5,6,7,8]
        Assert.Equal(7, set1.Count);
        Assert.DoesNotContain(1, set1);
        Assert.DoesNotContain(9, set1);

        (HashSet<int>, HashSet<int>) InitSets()
        {
            return (numbers.ToHashSet(), [1, 9]);
        }
    }

    [Fact]
    public void SortedSet()
    {
        int[] nums = [15, 1, 4, 12, 30, 1];
        var set = new SortedSet<int>(nums);

        Assert.Equal(5, set.Count);
        Assert.Equal(1, set.Min);
        Assert.Equal(30, set.Max);

        var subset = set.GetViewBetween(3, 16);
        Assert.True(subset.SetEquals([4, 12, 15]));

        subset = set.GetViewBetween(1, 15);
        Assert.True(subset.SetEquals([1, 4, 12, 15]));

        subset = set.GetViewBetween(-1, 555);
        Assert.True(subset.SetEquals([1, 4, 12, 15, 30]));

        subset = set.GetViewBetween(2, 3);
        Assert.True(subset.SetEquals([]));

        // Subtraction
        set.ExceptWith([15, 12]);
        Assert.True(set.SetEquals([1, 4, 30]));
    }


    [Fact]
    public void Stack()
    {
        var stk = new Stack<int>([2, 3, 1]);

        Assert.Equal(3, stk.Max());
        Assert.Equal(1, stk.Min());

        Assert.Equal(1, stk.Peek());
        Assert.Equal(3, stk.Count);

        Assert.Equal(1, stk.Pop());

        stk.Push(5);
        Assert.Equal(5, stk.Pop());
    }

    [Fact]
    public void ImmutableStack()
    {
        var rw = new Stack<int>([2, 3, 1]);
        var ro = System.Collections.Immutable.ImmutableStack.Create([2, 3, 1]);

        Assert.Equal(rw.Count, ro.Count());
        Assert.Equal(rw.Max(), ro.Max());
        Assert.Equal(rw.Min(), ro.Min());

        ro = ro.Pop(out var pop1);
        Assert.Equal(rw.Pop(), pop1);

        rw.Push(5);
        ro.Push(5).Pop(out var pop2);
        Assert.Equal(rw.Pop(), pop2);
    }

    [Fact]
    public void Queue()
    {
        var q = new Queue<int>([2, 3, 1]);

        Assert.Equal(2, q.Peek());
        Assert.Equal(3, q.Count);
        Assert.Equal(2, q.Dequeue());
        Assert.Equal(3, q.Dequeue());

        q.Enqueue(5);
        Assert.Equal(1, q.Dequeue());
        Assert.Equal(5, q.Peek());
    }

    [Fact]
    public void PriorityQueueMinHeap()
    {
        var minHeap = new PriorityQueue<string, int>();
        minHeap.Enqueue("A", 5);
        minHeap.Enqueue("B", 15);
        minHeap.Enqueue("C", 1);
        minHeap.Enqueue("D", 3);
        minHeap.Enqueue("E", 3);

        // "C" has the lowest priority and placed in the root of the MinHeap
        Assert.Equal("C", minHeap.Dequeue());
    }

    [Fact]
    public void PriorityQueueMaxHeap()
    {
        var maxHeap = new PriorityQueue<string, int>(Comparer<int>.Create((x, y) => y - x));
        maxHeap.Enqueue("A", 5);
        maxHeap.Enqueue("B", 15);
        maxHeap.Enqueue("C", 1);
        maxHeap.Enqueue("D", 3);
        maxHeap.Enqueue("E", 3);

        // "B" has the biggest priority and placed in the root of the MinHeap
        Assert.Equal("B", maxHeap.Dequeue());
    }

    [Fact]
    public void EnumerableTest()
    {
        var ienumerable1 = CollectionUtil.CreateEnumerable();
        // IEnumerable does not have Count so we apply Any() extension method
        Assert.True(ienumerable1.Any());

        var ienumerable2 = Enumerable.Range(1, 10).Select(x => x * x);
        Assert.Equal(100, ienumerable2.Last());
    }

    [Fact]
    public void IEnumerableTest()
    {
        var week = new Week();
        var sb1 = new StringBuilder();
        var sb2 = new StringBuilder();

        foreach (var day in week)
        {
            sb1.Append(day);
        }

        using var e = week.GetEnumerator();
        while (e.MoveNext())
        {
            sb2.Append(e.Current);
        }

        Assert.Equal(sb1.ToString(), sb2.ToString());
    }

    

    class WeekEnumerator(string[] days) : IEnumerator<string>
    {
        int position = -1;

        public string Current
        {
            get
            {
                if (position == -1 || position >= days.Length)
                    throw new ArgumentException();
                return days[position];
            }
        }
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (position < days.Length - 1)
            {
                position++;
                return true;
            }
            return false;
        }
        public void Reset() => position = -1;
        public void Dispose() { }
    }
    class Week
    {
        public IEnumerator<string> GetEnumerator() => new WeekEnumerator(["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]);
    }

    [Fact]
    public void ObservableCollection()
    {
        var family = new ObservableCollection<string> { "Victor", "Tatyana" };
        family.CollectionChanged += CollectionChanged;

        family.Add("Stasya");
        Assert.Equal(3, family.Count);

        family.RemoveAt(2);
        Assert.Equal(2, family.Count);

        family[0] = "Ira";
        Assert.Equal(2, family.Count);
        Assert.DoesNotContain("Victor", family);

        family.Move(0, 1);
        Assert.Equal("Tatyana", family[0]);
        Assert.Equal("Ira", family[1]);

        family.Clear();

        void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Assert.Equal("Stasya", e.NewItems?[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Assert.Equal("Stasya", e.OldItems?[0]);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Assert.Equal("Ira", e.NewItems?[0]);
                    Assert.Equal("Victor", e.OldItems?[0]);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Assert.Null(e.NewItems);
                    Assert.Null(e.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    Assert.Equal("Ira", e.NewItems?[0]);
                    Assert.Equal("Ira", e.OldItems?[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}