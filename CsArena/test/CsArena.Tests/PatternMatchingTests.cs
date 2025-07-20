using LinqArena;

namespace CsArena.Tests;

public class PatternMatchingTests
{
    [Fact]
    public void TypePattern()
    {
        string GetDescription(object? obj)
        {
            return obj switch
            {
                string => "string",
                int => "integer",
                null => "null",
                _ => "something else"
            };
        }

        Assert.Equal("integer", GetDescription(11));
        Assert.Equal("null", GetDescription(null));
        Assert.Equal("string", GetDescription("me"));
        Assert.Equal("something else", GetDescription(1.5));
    }

    [Fact]
    public void NestedPropertyPattern()
    {
        string GetCategory(object obj)
        {
            // Nested property pattern
            return obj switch
            {
                Car { Combined: >= 50 and < 60 } => "B",
                Car { Combined: >= 60 and < 70 } => "A",
                Car => "C",
                _ => "Unknown"
            };
        }

        Assert.Equal("B", GetCategory(new Car { Combined = 50 }));
        Assert.Equal("C", GetCategory(new Car { Combined = 10 }));
        Assert.Equal("Unknown", GetCategory("Try me"));
    }

    [Fact]
    public void DeclarationAndVarPattern()
    {
        int Rate(object obj)
        {
            return obj switch
            {
                // Declaration pattern
                Car { Combined: >= 50 and < 60 } c => c.Combined + 50,
                Car { Combined: >= 60 and < 70 } c => c.Combined - 50,
                // Var pattern
                var c => c.ToString()!.Length
            };
        }

        Assert.Equal(100, Rate(new Car { Combined = 50 }));
        Assert.Equal(10, Rate(new Car { Combined = 60 }));
        Assert.Equal(5, Rate("Hello"));
    }

    [Fact]
    public void PositionalPattern()
    {
        var c1 = new Car { Combined = 50, Name = "Accord", Manufacturer = "Honda" };
        Assert.True(c1 is (_, 50));
        Assert.True(c1 is ("Accord", 50));

        var c2 = new Car { Combined = 60, Name = "Accord", Manufacturer = "Honda" };
        Assert.True(c2 is ("Honda", _, 60));
    }

    [Fact]
    public void PositionalTuplePattern()
    {
        var tuple = (Combined: 50, Name: "Accord", Manufacturer: "Honda");

        Assert.True(tuple is ( > 10, _, _));
        Assert.True(tuple is (50, "Accord", _));
    }

    [Fact]
    public void LogicalAndRelationalPattern()
    {
        var c1 = new Car { Combined = 50, Name = "Accord", Manufacturer = "Honda" };

        Assert.True(c1 is { Combined: 50, Name.Length: 6 });
        Assert.True(c1 is { Combined: not 55, Name.Length: > 1 and <= 6 });
    }

    [Fact]
    public void CaseGuards()
    {
        var p1 = Point.Transform(new Point(0, 0));
        Assert.Equal((1, -1), (p1.X, p1.Y));

        var p2 = Point.Transform(new Point(0, 2));
        Assert.Equal((2, 2), (p2.X, p2.Y));

        var p3 = Point.Transform(new Point(2, 1));
        Assert.Equal((1, 1), (p3.X, p3.Y));

        var p4 = Point.Transform(new Point(5, 5));
        Assert.Equal((10, 10), (p4.X, p4.Y));
    }

    private record Point(int X, int Y)
    {
        public static Point Transform(Point point) => point switch
        {
            { X: 0, Y: 0 } => new Point(1, -1),
            // Property pattern with case guards ('when' keyword)
            { X: var x, Y: var y } when x < y => new Point(x + y, y),
            { X: var x, Y: var y } when x > y => new Point(x - y, y),
            { X: var x, Y: var y } => new Point(2 * x, 2 * y),
        };
    }

    [Fact]
    public void ListPattern()
    {
        int GetIndex(int[] arr) => arr switch
        {
            [2, 3, 5] => 1,
            [2, _, 5] => 2,
            [2, .., 5] => 3,
            [..] => 4
        };

        Assert.Equal(1, GetIndex([2, 3, 5]));
        Assert.Equal(2, GetIndex([2, 4, 5]));
        Assert.Equal(3, GetIndex([2, 2, 2, 2, 5]));
        Assert.Equal(4, GetIndex([1, 2, 3, 4, 5]));
    }
}