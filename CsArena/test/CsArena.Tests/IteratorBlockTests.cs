using CsArena.Tests.ext;

namespace CsArena.Tests;

public class IteratorBlockTests
{
    [Fact]
    public void YieldReturnTest()
    {
        var state = string.Empty;

        IEnumerable<int> GetInts()
        {
            state = "Before first yield";
            yield return 1;
            state = "Between yields";
            yield return 2;
            state = "After second yield";
        }

        using (var it = GetInts().GetEnumerator())
        {
            // No lines in GetInts() executed so far
            Assert.Equal("", state);

            // The first line in GetInts() was executed only on MoveNext() call
            Assert.True(it.MoveNext());
            Assert.Equal("Before first yield", state);
            Assert.Equal(1, it.Current);

            // GetInts() continued execution only after second MoveNext() call
            Assert.True(it.MoveNext());
            Assert.Equal("Between yields", state);
            Assert.Equal(2, it.Current);

            Assert.False(it.MoveNext());
            Assert.Equal("After second yield", state);

            // By design calling Current() multiple times will always return the value, stored on the last `yield return`
            Assert.Equal(2, it.Current);
            Assert.Equal(2, it.Current);
            Assert.Equal(2, it.Current);

            // By design Reset() throws NSE
            Assert.Throws<NotSupportedException>(it.Reset);
        }

        // foreach creates new Enumerator instance and iterates again through `yield return 1` and `yield return 2`
        foreach (var i in GetInts())
        {
            Assert.Contains(i, Enumerable.Range(1, 2));
        }
    }

    [Fact]
    public void YieldBreakTest()
    {
        IEnumerable<int> GetInts(int i)
        {
            while (true)
            {
                if (i == 5)
                    yield break;

                yield return i++;
            }
        }

        using (var it = GetInts(0).GetEnumerator())
        {
            Assert.True(it.MoveNext());
            Assert.Equal(0, it.Current);

            Assert.True(it.MoveNext());
            Assert.Equal(1, it.Current);

            Assert.True(it.MoveNext());
            Assert.Equal(2, it.Current);
        }

        using (var it = GetInts(5).GetEnumerator())
        {
            Assert.False(it.MoveNext());
            Assert.Equal(default(int), it.Current);
        }
    }

    [Fact]
    public void YieldIntInExtensionMethodTest()
    {
        var list = new List<int>(5);

        foreach (var i in 5)
        {
            list.Add(i);
        }
        Assert.Equal([0, 1, 2, 3, 4, 5], list);

        list.Clear();

        foreach (var i in -5)
        {
            list.Add(i);
        }
        Assert.Equal([-5, -4, -3, -2, -1, 0], list);
    }
}