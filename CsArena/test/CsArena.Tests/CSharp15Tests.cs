namespace CsArena.Tests;

// Union types (C# 15)
// A union value is exactly one of its case types; each case type converts implicitly to the union
// and a switch over the union is checked for exhaustiveness without a default arm.
internal record Circle(double Radius);
internal record Rect(double Width, double Height);
internal record Triangle(double Base, double Height);

internal union Shape(Circle, Rect, Triangle);

// Closed hierarchies (C# 15)
// A `closed` class can only be derived from within its own assembly, so the compiler knows every
// direct descendant and a switch that handles them all is exhaustive.
internal closed record GateState;
internal record GateClosed : GateState;
internal record GateOpen(float Percent) : GateState;

// Extension indexers (C# 15)
// An indexer is always an instance member, so the extension block needs a named receiver.
file static class SequenceIndexer
{
    extension(IEnumerable<int> sequence)
    {
        public int this[int index] => sequence.ElementAt(index);
    }
}

// Static members on interfaces (C# 15)
// A non-virtual static interface member is callable directly on the interface type.
file interface IMeasurement
{
    public static double ToMeters(double centimeters) => centimeters / 100;
}

public class CSharp15Tests
{
    [Fact]
    public void UnionTypeExhaustiveSwitchTest()
    {
        double Area(Shape shape) => shape switch
        {
            Circle c => Math.PI * c.Radius * c.Radius,
            Rect r => r.Width * r.Height,
            Triangle t => 0.5 * t.Base * t.Height,
            // No default arm: the compiler knows these three case types are the only possibilities.
        };
        
        // Implicit conversion from each case type to the union.
        Shape circle = new Circle(1);
        Shape rect = new Rect(3, 4);
        Shape triangle = new Triangle(6, 2);

        Assert.Equal(Math.PI, Area(circle), precision: 10);
        Assert.Equal(12, Area(rect));
        Assert.Equal(6, Area(triangle));
    }

    [Fact]
    public void UnionTypePatternMatchingTest()
    {
        Shape shape = new Rect(2, 5);

        Assert.True(shape is Rect);
        Assert.False(shape is Circle);
        Assert.True(shape is Rect { Width: 2 });
    }

    [Fact]
    public void ClosedHierarchyExhaustiveSwitchTest()
    {
        string Describe(GateState state) => state switch
        {
            GateClosed => "closed",
            GateOpen(var percent) => $"{percent}% open",
            // No default arm: every direct descendant of the closed class is handled.
        };

        Assert.Equal("closed", Describe(new GateClosed()));
        Assert.Equal("50% open", Describe(new GateOpen(50)));
    }

    [Fact]
    public void ExtensionIndexerTest()
    {
        IEnumerable<int> numbers = Enumerable.Range(1, 10);

        Assert.Equal(1, numbers[0]);
        Assert.Equal(3, numbers[2]);
        Assert.Equal(10, numbers[9]);
        Assert.Throws<ArgumentOutOfRangeException>(() => numbers[10]);
    }

    [Fact]
    public void LabeledContinueTest()
    {
        int[][] rows = [[1, 2, 3], [4, -1, 6], [7, 8, 9]];
        var sum = 0;

        // `continue outer` abandons the current row as soon as a negative number is found,
        // replacing a Boolean flag checked after the inner loop.
        outer: foreach (var row in rows)
        {
            foreach (var n in row)
            {
                if (n < 0) 
                    continue outer;
                sum += n;
            }
        }

        // Row 2 contributes only its first element (4) before -1 skips the rest of it.
        Assert.Equal(1 + 2 + 3 + 4 + 7 + 8 + 9, sum);
    }

    [Fact]
    public void LabeledBreakTest()
    {
        int[][] grid = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        (int Row, int Col)? found = null;

        // `break search` exits both loops at once, replacing a `goto` or a flag.
        search: for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] != 5) 
                    continue;
                found = (row, col);
                break search;
            }
        }

        Assert.Equal((1, 1), found);
    }

    [Fact]
    public void CollectionExpressionArgumentsTest()
    {
        string[] values = ["one", "two", "three"];

        // with(...) must be the first element; its arguments go to the collection's constructor.
        List<string> names = [with(capacity: values.Length * 2), .. values];
        Assert.Equal(values, names);
        Assert.Equal(values.Length * 2, names.Capacity);

        HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
        Assert.Single(set);
    }

    [Fact]
    public void StaticInterfaceMemberTest()
    {
        Assert.Equal(2.5, IMeasurement.ToMeters(250));
    }
}