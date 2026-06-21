using System.Numerics;
using CsArena.Tests.models;

namespace CsArena.Tests;

public static class CSharp14Feature
{
    // The members in the first extension block are called as though they're instance members of any number
    extension<T>(T i) where T : INumber<T>
    {
        public T Square() => i * i;
        public T Cube() => i * i * i;
    }

    // The members in the second extension block are called as though they're static members of any number
    extension<T>(T) where T : INumber<T>
    {
        public static T Pow2(T i) => i * i;
        public static T Pow3(T i) => i * i * i;
    }
}

// field keyword (C# 14): references the compiler-generated backing field inside a property
// accessor, enabling semi-auto properties without an explicit backing field declaration.
file class Temperature
{
    public double Celsius
    {
        get;
        set => field = value >= -273.15
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Celsius), "Below absolute zero");
    }
}

public class CSharp14Tests
{
    [Fact]
    public void FieldKeywordTest()
    {
        var t = new Temperature { Celsius = 100.0 };
        Assert.Equal(100.0, t.Celsius);
        Assert.Throws<ArgumentOutOfRangeException>(() => t.Celsius = -300.0);
    }

    // params ReadOnlySpan<T> (C# 13): stack-allocates the argument list — zero heap pressure.
    private static int SumInts(params ReadOnlySpan<int> numbers)
    {
        var total = 0;
        foreach (var n in numbers) total += n;
        return total;
    }

    [Fact]
    public void ParamsSpanTest()
    {
        Assert.Equal(15, SumInts(1, 2, 3, 4, 5));
        Assert.Equal(0,  SumInts());
        Assert.Equal(42, SumInts(42));
    }

    [Fact]
    public void NullConditionalAssignmentTest()
    {
        string Rhs()
        {
            Assert.True(false);
            return "NullConditionalAssignment";
        }

        // If book is null, the code doesn't call the assignment and RHS is not evaluated
        Book? book = null;
        book?.Name = Rhs();
        Assert.Null(book);
    }

    [Fact]
    public void ExtensionBlockTest()
    {
        Assert.Equal(9, 3.Square());
        Assert.Equal(27, 3.Cube());
        Assert.Equal(9.0f, 3.0f.Square());
        Assert.Equal(9.0d, 3.0d.Square());
        Assert.Equal(9.0m, 3.0m.Square());
        Assert.Equal(25u, 5u.Square());
        Assert.Equal(125u, 5u.Cube());

        Assert.Equal(9, int.Pow2(3));
        Assert.Equal(27, int.Pow3(3));
    }
}